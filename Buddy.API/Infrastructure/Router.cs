using Buddy.API.Model;
using Buddy.Utilities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Xml;

namespace Buddy.API.Infrastructure
{
    public class Router
    {
        readonly Helper helper = new Helper();
        readonly IConfiguration config;

        public Router(IConfiguration iConfig)
        {
            config = iConfig;
        }
        public Response Execute(ServiceMetaData serviceMetaData, dynamic serviceParam)
        {
            Response response = InitializeResponse();
            ClassInfo modelInfo = new ClassInfo();
            if (!string.IsNullOrEmpty(serviceMetaData.ModelClass))
            {
                modelInfo = ReflectAssemblyClass(serviceMetaData.ModelClass, null, ref response);
                if (response.Code != 0)
                    return response;
            }

            ClassInfo serviceInfo = ReflectAssemblyClass(serviceMetaData.AssemblyClass, null, ref response);
            if (response.Code != 0)
                return response;

            MethodInfo methodInfo = serviceInfo.ClassType.GetMethod(serviceMetaData.MethodName);
            if (methodInfo == null)
                response.Messages.Add("Method not found in class (" + serviceMetaData.MethodName + ")");
            else
            {
                ParameterInfo[] parameterInfo = methodInfo.GetParameters();
                if (parameterInfo.Length > 0 && serviceParam == null)
                    response.Messages.Add("Method (" + serviceMetaData.MethodName + ") expecting parameters");
                else
                {
                    object parameter = new object();
                    if (parameterInfo.Length > 0)
                    {
                        if (parameterInfo.Length > 1)
                            response.Messages.Add("Method (" + serviceMetaData.MethodName + ") shouldn't have more than one parameter");
                        else
                        {
                            if (string.IsNullOrEmpty(serviceMetaData.ModelClass))
                            {
                                // handle generic parameter
                                parameter = Convert.ChangeType(serviceParam, parameterInfo[0].ParameterType);
                            }
                            else
                            {
                                // handle model parameter
                                if (modelInfo.AssemblyType != null)
                                {
                                    JObject paramJObject = JObject.Parse(serviceParam);
                                    parameter = helper.MapObjects(paramJObject, modelInfo.ClassInstance);
                                }
                                else
                                    response.Messages.Add("There is a conflict between service parameter type and the provided model class type");
                            }
                        }
                    }
                    response.Results = methodInfo.Invoke(serviceInfo.ClassInstance, new object[] { parameter });
                }
            }
            return response;
        }

        /// <summary>
        /// reflect assembly name and class to activate while compiling and return object of class info
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="ctorParameters"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public ClassInfo ReflectAssemblyClass(string assembly, object ctorParameters, ref Response response)
        {
            ClassInfo classInfo = new ClassInfo();
            response.Code = 0;
            string[] assemblySections = assembly.Split(',');
            string assemblyName = assemblySections[0];
            string className = assemblySections[1];
            if (string.IsNullOrEmpty(assemblyName) || string.IsNullOrEmpty(className))
            {
                response.Code = -1;
                response.Messages.Add("Invalid assembly value (" + assembly + ")");
            }
            classInfo.AssemblyType = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Combine(helper.GetApplicationPath(), "bin", assemblyName + ".dll"));
            classInfo.ClassType = classInfo.AssemblyType.GetType(assembly.Replace(",", "."));
            classInfo.ClassInstance = Activator.CreateInstance(classInfo.ClassType, ctorParameters == null ? null : new object[] { ctorParameters });
            if (classInfo.ClassType == null)
            {
                response.Code = -1;
                response.Messages.Add("Class not found in assembly (" + assembly.Replace(",", ".") + ")");
            }
            if (classInfo.ClassInstance == null)
            {
                response.Code = -1;
                response.Messages.Add("Counldn't get an instance of class (" + className + ")");
            }

            return classInfo;
        }

        /// <summary>
        /// get service settings data by service code from RouterData.xml file
        /// </summary>
        /// <param name="serviceCode"></param>
        /// <param name="serviceConfigurationError"></param>
        /// <returns></returns>
        public ServiceMetaData GetServiceData(string serviceCode, ref string serviceConfigurationError)
        {
            string routerDataFileRelativePath = config.GetSection("BuddySettings").GetSection("routerDataFileRelativePath").Value;
            XmlDocument routerXML = helper.ReadXMLFile(routerDataFileRelativePath);
            XmlNodeList ServiceList = routerXML.SelectNodes("/Buddy/Services/Service[@Code='" + serviceCode + "']");
            ServiceMetaData serviceMetaData = new ServiceMetaData();
            if (ServiceList[0] != null && ServiceList[0].Attributes.Count > 0)
            {
                if (ServiceList[0].Attributes["AssemblyClass"] == null || ServiceList[0].Attributes["Method"] == null)
                    serviceConfigurationError = "Assembly or Method is null or empty configured";
                else
                {
                    serviceMetaData = new ServiceMetaData()
                    {
                        AssemblyClass = ServiceList[0].Attributes["AssemblyClass"].Value.ToString(),
                        MethodName = ServiceList[0].Attributes["Method"].Value.ToString(),
                        ModelClass = ServiceList[0].Attributes["ModelClass"].Value.ToString(),
                        AccessType = Convert.ToInt32(ServiceList[0].Attributes["AccessType"].Value.ToString()),
                        CashingType = Convert.ToInt32(ServiceList[0].Attributes["CashingType"].Value.ToString())
                    };
                }
            }
            else
                serviceConfigurationError = "Service code is not found";

            return serviceMetaData;
        }

        /// <summary>
        /// initialize response object with default data
        /// </summary>
        /// <returns></returns>
        public Response InitializeResponse()
        {
            return new Response()
            {
                Code = 0,
                Messages = new List<string>(),
                Results = new Object()
            };
        }
    }
}
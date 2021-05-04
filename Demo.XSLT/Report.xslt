<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" omit-xml-declaration="yes" indent="yes"/>
	<xsl:template match="*">
		<ServiceOrderCollection>
			<ServiceOrderItem>
				<ProductType>credit</ProductType>
				<IsSimulation>false</IsSimulation>
				<RequestXml>
					<CreditReportItem>
						<CreditConfigItem>
							<ProductName>
								<xsl:value-of select="//ProductName"/>
							</ProductName>
							<IsPrecheckRequired>
								<xsl:choose>
									<xsl:when test="//IsPrecheckRequired !='''' and //IsPrecheckRequired =''Y''">
										<xsl:text>true</xsl:text>
									</xsl:when>
									<xsl:otherwise>
										<xsl:text>false</xsl:text>
									</xsl:otherwise>
								</xsl:choose>
							</IsPrecheckRequired>
							<IsCreditOrderNextDateResetRequired>
								<xsl:choose>
									<xsl:when test="//IsCreditOrderNextDateResetRequired !='''' and //IsCreditOrderNextDateResetRequired =''Y''">
										<xsl:text>true</xsl:text>
									</xsl:when>
									<xsl:otherwise>
										<xsl:text>false</xsl:text>
									</xsl:otherwise>
								</xsl:choose>
							</IsCreditOrderNextDateResetRequired>
						</CreditConfigItem>
						<ServiceType>credit</ServiceType>
						<CreditReportType>Merge</CreditReportType>
						<xsl:choose>
							<xsl:when test="//ServicerID != 50486 and //ServicerID != 1164413">
								<IncludePDF>true</IncludePDF>
							</xsl:when>
							<xsl:otherwise>
								<IncludePDF>false</IncludePDF>
							</xsl:otherwise>
						</xsl:choose>
						<xsl:choose>
							<!--xsl:when test="//ContextIdentifierLoanID != '''' and //ServicerID = 1133792"> <ProductType>Reissue</ProductType> </xsl:when-->
							<xsl:when test="//ContextIdentifierLoanID != '''' and //ServicerID = 1365456">
								<ProductType>Reissue</ProductType>
							</xsl:when>
							<!--xsl:when test ="//ContextIdentifierLoanID != '''' and //ServicerID = 1060742"> <ProductType>Reissue</ProductType> </xsl:when>-->
							<xsl:otherwise>
								<ProductType>Submit</ProductType>
							</xsl:otherwise>
						</xsl:choose>
						<Identifier>
							<xsl:choose>
								<xsl:when test="//ContextIdentifierLoanID != '''' and //ServicerID = 1133792">
									<xsl:value-of select="//ContextIdentifierLoanID"/>
								</xsl:when>
								<xsl:when test="//ContextIdentifierLoanID != '''' and //ServicerID = 1365456">
									<xsl:value-of select="//ContextIdentifierLoanID"/>
								</xsl:when>
								<!--xsl:when test ="//ContextIdentifierLoanID != '''' and //ServicerID = 1060742"> <xsl:value-of select ="//ContextIdentifierLoanID"/> </xsl:when>-->
							</xsl:choose>
						</Identifier>
						<BureauIndicator>
							<TransUnionIndicator>
								<xsl:value-of select="//EquifaxIndicatorItemItem/TransunionIndicator"/>
							</TransUnionIndicator>
							<xsl:if test="//EquifaxIndicatorItemItem/EquifaxIndicator">
								<EquifaxIndicator>
									<xsl:value-of select="//EquifaxIndicatorItemItem/EquifaxIndicator"/>
								</EquifaxIndicator>
							</xsl:if>
							<xsl:if test="//EquifaxIndicatorItemItem/ExperianIndicator">
								<ExperianIndicator>
									<xsl:value-of select="//EquifaxIndicatorItemItem/ExperianIndicator"/>
								</ExperianIndicator>
							</xsl:if>
							<xsl:if test="//EquifaxIndicatorItemItem/CreditFICO">
								<CreditFICO>
									<xsl:value-of select="//EquifaxIndicatorItemItem/CreditFICO"/>
								</CreditFICO>
							</xsl:if>
							<xsl:if test="//EquifaxIndicatorItemItem/EquifaxModel">
								<EquifaxModel>
									<xsl:value-of select="//EquifaxIndicatorItemItem/EquifaxModel"/>
								</EquifaxModel>
							</xsl:if>
							<xsl:if test="//EquifaxIndicatorItemItem/ExperianModel">
								<ExperianModel>
									<xsl:value-of select="//EquifaxIndicatorItemItem/ExperianModel"/>
								</ExperianModel>
							</xsl:if>
							<xsl:if test="//EquifaxIndicatorItemItem/TransunionModel">
								<TransunionModel>
									<xsl:value-of select="//EquifaxIndicatorItemItem/TransunionModel"/>
								</TransunionModel>
							</xsl:if>
						</BureauIndicator>
						<Object>Loan</Object>
						<ObjectID>
							<xsl:value-of select="//ObjectID"/>
						</ObjectID>
						<CheckIfExists>True</CheckIfExists>
						<OrganizationID>
							<xsl:value-of select="//ServicerID"/>
						</OrganizationID>
						<ClientName>
							<xsl:choose>
								<xsl:when test="//ClientName">
									<xsl:value-of select="//ClientName"/>
								</xsl:when>
								<xsl:otherwise>Test</xsl:otherwise>
							</xsl:choose>
						</ClientName>
						<Address>
							<xsl:value-of select="//Address"/>
						</Address>
						<City>
							<xsl:value-of select="//City"/>
						</City>
						<State>
							<xsl:value-of select="//State"/>
						</State>
						<PostalCode>
							<xsl:value-of select="//PostalCode"/>
						</PostalCode>
						<BorrowerCollection>
							<xsl:if test="count(//PrimaryBorrowerUSSSN) > 0">
								<PrimaryBorrower>
									<PrimaryBorrowerID>
										<xsl:value-of select="//PrimaryBorrowerID"/>
									</PrimaryBorrowerID>
									<PrimaryBorrowerContactID>
										<xsl:value-of select="//PrimaryBorrowerContactID"/>
									</PrimaryBorrowerContactID>
									<FirstName>
										<xsl:value-of select="//PrimaryBorrowerFirstName"/>
									</FirstName>
									<LastName>
										<xsl:value-of select="//PrimaryBorrowerLastName"/>
									</LastName>
									<USSSN>
										<xsl:value-of select="//PrimaryBorrowerUSSSN"/>
									</USSSN>
								</PrimaryBorrower>
							</xsl:if>
							<xsl:if test="count(//SecondaryBorrowerUSSSN) > 0">
								<SecondaryBorrower>
									<SecondaryBorrowerID>
										<xsl:value-of select="//SecondaryBorrowerID"/>
									</SecondaryBorrowerID>
									<SecondaryBorrowerContactID>
										<xsl:value-of select="//SecondaryBorrowerContactID"/>
									</SecondaryBorrowerContactID>
									<FirstName>
										<xsl:value-of select="//SecondaryBorrowerFirstName"/>
									</FirstName>
									<LastName>
										<xsl:value-of select="//SecondaryBorrowerLastName"/>
									</LastName>
									<USSSN>
										<xsl:value-of select="//SecondaryBorrowerUSSSN"/>
									</USSSN>
								</SecondaryBorrower>
							</xsl:if>
						</BorrowerCollection>
					</CreditReportItem>
				</RequestXml>
			</ServiceOrderItem>
		</ServiceOrderCollection>
	</xsl:template>
</xsl:stylesheet>
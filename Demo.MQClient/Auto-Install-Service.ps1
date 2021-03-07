# sc.exe Delete MQClientService

$MQServiceName = "MQClientService"
$MQPath = 'C:\Users\mohamed_behery\Desktop\My Partition\Free Time\Buddy\Demo.MQClient\bin\Release\MQClient.exe'
$MQProperities = @{
    Name = $MQServiceName
    BinaryPathName = $MQPath
    DisplayName = $MQServiceName
    StartupType = "Automatic"
    Description = "This is a MQ client service."
}

# check if service already installed then stop it
If (Get-Service $MQServiceName -ErrorAction SilentlyContinue) {
    If ((Get-Service $MQServiceName).Status -eq 'Running') {
        Stop-Service $MQServiceName
        Write-Host "Stopping $MQServiceName..."
        $ServiceFound = Get-WmiObject -Class Win32_Service -Filter "Name='$MQServiceName'"
        $ServiceFound.delete()
    }
}

# install service
New-Service @MQProperities
Start-Service -Name MQClientService
# SC Delete RabbitMQClientService

$RabbitMQServiceName = "RabbitMQClientService"
$RabbitMQPath = 'C:\Users\mohamed_behery\Desktop\My Partition\Free Time\Buddy\Demo.RabbitMQClientWinService\bin\Release\RabbitMQClientWinService.exe'
$RabbitMQProperities = @{
    Name = $RabbitMQServiceName
    BinaryPathName = $RabbitMQPath
    DisplayName = $RabbitMQServiceName
    StartupType = "Automatic"
    Description = "This is a RabbitMQ client service."
}

# check if service already installed then stop it
If (Get-Service $RabbitMQServiceName -ErrorAction SilentlyContinue) {
    If ((Get-Service $RabbitMQServiceName).Status -eq 'Running') {
        Stop-Service $RabbitMQServiceName
        Write-Host "Stopping $RabbitMQServiceName..."
        $ServiceFound = Get-WmiObject -Class Win32_Service -Filter "Name='$RabbitMQServiceName'"
        $ServiceFound.delete()
    }
}

# install service
New-Service @RabbitMQProperities
Start-Service -Name RabbitMQClientService
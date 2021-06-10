# sc.exe Delete FileWatcherService

$FileWatcherServiceName = "FileWatcherService"
$FileWatcherPath = "C:\Users\mohamed_behery\Desktop\My Partition\Free Time\Buddy\Demo.FileWatcher\bin\Release\FileWatcher.exe"
$FileWatcherProperities = @{
    Name = $FileWatcherServiceName
    BinaryPathName = $FileWatcherPath
    DisplayName = $FileWatcherServiceName
    StartupType = "Automatic"
    Description = "This is a file watcher service."
}

# check if service already installed then stop it
If (Get-Service $FileWatcherServiceName -ErrorAction SilentlyContinue) {
    If ((Get-Service $FileWatcherServiceName).Status -eq 'Running') {
        Stop-Service $FileWatcherServiceName
        Write-Host "Stopping $FileWatcherServiceName..."
        $ServiceFound = Get-WmiObject -Class Win32_Service -Filter "Name='$FileWatcherServiceName'"
        $ServiceFound.delete()
    }
}

# install service
New-Service @FileWatcherProperities
Start-Service -Name FileWatcherService
try {
    $cake = Invoke-WebRequest https://cakebuild.net/download/bootstrapper/windows -OutFile build.ps1 -PassThru
    if ($cake.statuscode -eq 200) {
        Write-Host -ForegroundColor Green "Obtained latest Version of Cake"
    }
} catch {
    Write-Warning -Message "Error obtaining latest Cake Bootstrapper"
    Write-Warning -Message $_.Exception.Message
    Write-Warning -Message "Switching to Backup-Bootstrapper"
    Copy-item .\build\build.ps1 .\
}
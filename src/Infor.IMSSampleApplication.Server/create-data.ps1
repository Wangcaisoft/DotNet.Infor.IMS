# Create Data\MessagePayload folder and grant current user write permissions
$path = Join-Path $PSScriptRoot "Data\MessagePayload"
if (-Not (Test-Path -Path $path)) {
    New-Item -ItemType Directory -Force -Path $path | Out-Null
    Write-Host "Created folder: $path"
} else {
    Write-Host "Folder already exists: $path"
}
# Grant full control to the current user (useful for debugging locally)
try {
    $account = [System.Security.Principal.WindowsIdentity]::GetCurrent().Name
    icacls $path /grant "$account:(OI)(CI)F" | Out-Null
    Write-Host "Granted full control to $account on $path"
} catch {
    Write-Warning "Failed to set ACLs: $_"
}

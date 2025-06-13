# setup DEVDEBUG conditional compilation: mockup authentication service with UiDevAuthenticationController
# run this with adminisrator's priveleges

# set
# [System.Environment]::SetEnvironmentVariable("ENV_DEVDEBUG", "DEVDEBUG", "Machine")

# check
# [System.Environment]::GetEnvironmentVariable("ENV_DEVDEBUG", "Machine")

# remove
# [System.Environment]::SetEnvironmentVariable("ENV_DEVDEBUG", $null, "Machine")


# Check if script is running as administrator
$IsAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $IsAdmin) {
    Write-Host "Not running as admin. Relaunching with elevated privileges..."

    # Relaunch script with elevated rights
    Start-Process powershell -Verb runAs -ArgumentList "-NoProfile -ExecutionPolicy Bypass -File `"$PSCommandPath`""

    exit  # Exit the current non-admin script
}

# If we're here, we ARE running as admin
Write-Host "Running as Administrator."

# Now do admin-required actions
[System.Environment]::SetEnvironmentVariable("ENV_DEVDEBUG", "DEVDEBUG", "Machine")
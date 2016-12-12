# Somitemes Asp.Core packages and Visual Studio 2015  can break down and show "Bad Reference" modal message.
# For repairing it try to re-reference "project references" separately (in asp.core project) or together with 
# "clear solution". This script is part of "Clear" functionality

$LocalAppDataFolder = [Environment]::GetFolderPath("LocalApplicationData")
$WebSiteCacheFoler = $LocalAppDataFolder + "\Microsoft\WebSiteCache"+"\*"
Remove-Item $WebSiteCacheFoler

# TODO: 
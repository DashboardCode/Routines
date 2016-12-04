$LocalAppDataFolder = [Environment]::GetFolderPath("LocalApplicationData")
$WebSiteCacheFoler = $LocalAppDataFolder + "\Microsoft\WebSiteCache"+"\*"
Remove-Item $WebSiteCacheFoler

# delete all except App_Data folder and its content
# D:\cot\Vse\AdminkaV1\Web\bin\Debug\net461\win7-x64

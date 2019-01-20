# in case of "cannot be loaded because running scripts is disabled on this system" execute 'Set-ExecutionPolicy RemoteSigned'
# in case of "Requested registry access is not allowed." run script in ISE started "as Administrator"

$categoryName = "DashboardCode Adminka"
$categoryHelp = "Application performance"
$categoryType = [System.Diagnostics.PerformanceCounterCategoryType]::MultiInstance

$isCategoryExists = [System.Diagnostics.PerformanceCounterCategory]::Exists($categoryName)
If ($isCategoryExists) {[System.Diagnostics.PerformanceCounterCategory]::Delete($categoryName);}

$counterCreationData1 = New-Object System.Diagnostics.CounterCreationData
$counterCreationData1.CounterName = "Avg. sec/action"
$counterCreationData1.CounterType = "AverageTimer32"

$counterCreationData2 = New-Object System.Diagnostics.CounterCreationData
$counterCreationData2.CounterName = "Avg. sec/action base"
$counterCreationData2.CounterType = "AverageBase"

$counterCreationData3 = New-Object System.Diagnostics.CounterCreationData
$counterCreationData3.CounterName = "number of actions"
$counterCreationData3.CounterType = "CounterDelta32"

$counterCreationData4 = New-Object System.Diagnostics.CounterCreationData
$counterCreationData4.CounterName = "Errors"
$counterCreationData4.CounterType = "CounterDelta32"

$collection = New-Object System.Diagnostics.CounterCreationDataCollection    
$collection.Add($counterCreationData1) | Out-Null
$collection.Add($counterCreationData2) | Out-Null
$collection.Add($counterCreationData3) | Out-Null
$collection.Add($counterCreationData4) | Out-Null

[System.Diagnostics.PerformanceCounterCategory]::Create($categoryName, $categoryHelp, $categoryType, $collection) | Out-Null

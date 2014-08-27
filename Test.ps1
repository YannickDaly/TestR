param (
    [Parameter()]
    [switch] $SlowMotion,
    [Parameter()]
    [switch] $AutoClose,
    [Parameter()]
    [TestR.BrowserType] $BrowserType
)

$tests = Get-Command -Module TestR.IntegrationTests
$arguments = ""

if ($RandomOrder) {
    $tests = $tests | Get-Random -Count $tests.Count
}

if ($AutoClose) {
    $arguments += " -AutoClose"
}

if ($SlowMotion) {
    $arguments += " -SlowMotion"
}

if ($BrowserType) {
    $arguments += " -BrowserType " + $BrowserType
}

if ($PSBoundParameters.ContainsKey("Verbose")){
    $arguments += " -Verbose"
}

foreach ($test in $tests) 
{
    try
    {
        $testNames = Invoke-Expression "$test"
        foreach ($testName in $testNames)
        {
            Write-Host "$test.$testName ..." -NoNewline
            $commandArguments = "-Name $testName" + $arguments
            Invoke-Expression "$test $commandArguments"
            Write-Host " Passed" -ForegroundColor Green
        }
    }
    catch [System.Exception]
    {
        Write-Host
        Write-Host " Failed:" $_ -ForegroundColor Red
        break
    }
}

[TestR.Browser]::CloseAllBrowsers([TestR.BrowserType]::All)
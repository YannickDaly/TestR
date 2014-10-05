param (
    [Parameter()]
    [switch] $SlowMotion = $false,
    [Parameter()]
    [switch] $RandomOrder = $false,
    [Parameter()]
    [TestR.BrowserType] $BrowserType = [TestR.BrowserType]::InternetExplorer,
    [Parameter()]
    [TestR.Logging.ILogger] $Logger
)

$tests = Get-Command -Module TestR.IntegrationTests

if ($RandomOrder) {
    $tests = $tests | Get-Random -Count $tests.Count
}

foreach ($test in $tests) 
{
    try
    {
        $testNames = Invoke-Expression "$test"
        if ($RandomOrder) {
            $testNames = $testNames | Get-Random -Count $testNames.Count
        }

        foreach ($testName in $testNames)
        {
            Write-Host "$test.$testName ..." -NoNewline
            & "$test" -Name $testName -SlowMotion:$SlowMotion -Logger:$Logger -BrowserType:$BrowserType -Verbose:$PSBoundParameters.ContainsKey("Verbose")
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
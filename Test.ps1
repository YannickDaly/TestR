param (
    [Parameter(Mandatory = $false)]
    [switch] $SlowMotion,
    [Parameter(Mandatory = $false)]
    [switch] $AutoClose,
    [Parameter(Mandatory = $false)]
    [switch] $RandomOrder
)

Import-Module TestR.IntegrationTests

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
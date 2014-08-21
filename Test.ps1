param (
    [Parameter(Mandatory = $false)]
    [switch] $SlowMotion,
    [Parameter(Mandatory = $false)]
    [switch] $RandomOrder
)

Import-Module TestR.PowerShell.Tests -Force

if ($SlowMotion) {
    $argumentList = "-SlowMotion"
}

$tests = Get-Command -Module TestR.PowerShell.Tests 
$arguments = ""

if ($RandomOrder) {
    $tests = $tests | Get-Random -Count $tests.Count
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
        Write-Host $test.Name ... -NoNewline
        Invoke-Expression "$test $arguments"
        Write-Host " Passed" -ForegroundColor Green
    }
    catch [System.Exception]
    {
        Write-Host " Failed:" $_ -ForegroundColor Red
        break
    }
}
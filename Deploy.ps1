param (
    [Parameter()]
    [switch] $IncludeDocumentation
)

$watch = [System.Diagnostics.Stopwatch]::StartNew()
$scriptPath = Split-Path (Get-Variable MyInvocation).Value.MyCommand.Path 
Set-Location $scriptPath
$destination = "C:\Binaries\TestR"
$nugetDestination = "C:\Workspaces\Nuget\Developer"

if (Test-Path $destination -PathType Container){
    Remove-Item $destination -Recurse -Force
}

New-Item $destination -ItemType Directory | Out-Null

if (!(Test-Path $nugetDestination -PathType Container)){
    New-Item $nugetDestination -ItemType Directory | Out-Null
}

$configuration = "Release"
if ($args.Count -gt 0) {
	$configuration = $args
}

$build = [Math]::Floor([DateTime]::UtcNow.Subtract([DateTime]::Parse("01/01/2000").Date).TotalDays)
$revision = [Math]::Floor([DateTime]::UtcNow.TimeOfDay.TotalSeconds / 2)

.\IncrementVersion.ps1 TestR\TestR $build $revision
.\IncrementVersion.ps1 TestR\TestR.IntegrationTests $build $revision
.\IncrementVersion.ps1 TestR\TestR.PowerShell $build $revision
.\IncrementVersion.ps1 TestR\TestR.UnitTests $build $revision

$msbuild = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe"
cmd /c $msbuild "$scriptPath\TestR\TestR.sln" /p:Configuration="$configuration" /p:Platform="Any CPU" /t:Rebuild /p:VisualStudioVersion=13.0 /v:m /m

if ($IncludeDocumentation) {
    cmd /c $msbuild "$scriptPath\TestR\TestR.shfbproj" /p:Configuration="$configuration" /p:Platform="Any CPU" /t:Rebuild /p:VisualStudioVersion=13.0 /v:m /m
}

Set-Location $scriptPath

Copy-Item TestR\TestR\bin\$configuration\TestR.dll $destination
Copy-Item TestR\TestR\bin\$configuration\Interop.SHDocVw.dll $destination
Copy-Item TestR\TestR.PowerSHell\bin\$configuration\TestR.PowerShell.dll $destination

$versionInfo = [System.Diagnostics.FileVersionInfo]::GetVersionInfo("$destination\TestR.dll")
$version = $versionInfo.FileVersion.ToString()

cmd /c "TestR\.nuget\NuGet.exe" pack TestR.nuspec -Prop Configuration="$configuration" -Version $version
Move-Item "TestR.$version.nupkg" "$destination" -force
Copy-Item "$destination\TestR.$version.nupkg" "$nugetDestination" -force

.\ResetAssemblyInfos.ps1

$modulesPath = "C:\Workspaces\PowerShell"
if (Test-Path $modulesPath -PathType Container) {
    Remove-Item $modulesPath -Force -Recurse
}

New-Item $modulesPath -ItemType Directory | Out-Null
$modules = "TestR.Powershell", "TestR.IntegrationTests", "TestR.UnitTests"

foreach ($module in $modules) {
    $modulePath = "$modulesPath\$module"
    if (Test-Path $modulePath -PathType Container) {
        Remove-Item $modulePath -Force -Recurse
    }

    $sourcePath = "C:\Workspaces\GitHub\TestR\TestR\$module\bin\$configuration\*"
    New-Item $modulePath -ItemType Directory | Out-Null
    Copy-Item $sourcePath $modulePath\ -Recurse -Force
}

Write-Host
Set-Location $scriptPath
Write-Host "TestR Deploy: " $watch.Elapsed
param (
    [Parameter()]
    [switch] $IncludeDocumentation,
    [Parameter()]
    [string] $Configuration = "Release"
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
New-Item $destination\bin -ItemType Directory | Out-Null

if (!(Test-Path $nugetDestination -PathType Container)){
    New-Item $nugetDestination -ItemType Directory | Out-Null
}

$build = [Math]::Floor([DateTime]::UtcNow.Subtract([DateTime]::Parse("01/01/2000").Date).TotalDays)
$revision = [Math]::Floor([DateTime]::UtcNow.TimeOfDay.TotalSeconds / 2)

.\IncrementVersion.ps1 TestR $build $revision
.\IncrementVersion.ps1 TestR.IntegrationTests $build $revision
.\IncrementVersion.ps1 TestR.PowerShell $build $revision
.\IncrementVersion.ps1 TestR.UnitTests $build $revision

$msbuild = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe"
cmd /c $msbuild "$scriptPath\TestR.sln" /p:Configuration="$Configuration" /p:Platform="Any CPU" /t:Rebuild /p:VisualStudioVersion=12.0 /v:m /m

if ($IncludeDocumentation) {
    cmd /c $msbuild "$scriptPath\TestR.shfbproj" /p:Configuration="$Configuration" /p:Platform="Any CPU" /t:Rebuild /p:VisualStudioVersion=12.0 /v:m /m
}

Set-Location $scriptPath

Copy-Item TestR\bin\$Configuration\TestR.dll $destination\bin\
Copy-Item TestR\bin\$Configuration\Interop.SHDocVw.dll $destination\bin\
Copy-Item TestR.Data\bin\$Configuration\TestR.Data.dll $destination\bin\
Copy-Item TestR.PowerShell\bin\$Configuration\TestR.PowerShell.dll $destination\bin\
Copy-Item Help\Documentation.chm $destination

$versionInfo = [System.Diagnostics.FileVersionInfo]::GetVersionInfo("$destination\bin\TestR.dll")
$version = $versionInfo.FileVersion.ToString()

cmd /c ".nuget\NuGet.exe" pack TestR.nuspec -Prop Configuration="$Configuration" -Version $version
Move-Item "TestR.$version.nupkg" "$destination" -force
Copy-Item "$destination\TestR.$version.nupkg" "$nugetDestination" -force

cmd /c ".nuget\NuGet.exe" pack TestR.PowerShell.nuspec -Prop Configuration="$Configuration" -Version $version
Move-Item "TestR.PowerShell.$version.nupkg" "$destination" -force
Copy-Item "$destination\TestR.PowerShell.$version.nupkg" "$nugetDestination" -force

.\ResetAssemblyInfos.ps1

$modulesPath = "C:\Workspaces\PowerShell"
if (!(Test-Path $modulesPath -PathType Container)) {
    New-Item $modulesPath -ItemType Directory | Out-Null
}

$modules = "TestR.Powershell", "TestR.IntegrationTests", "TestR.UnitTests"

foreach ($module in $modules) {
    $modulePath = "$modulesPath\$module"
    if (Test-Path $modulePath -PathType Container) {
        Remove-Item $modulePath -Force -Recurse
    }

    $sourcePath = "C:\Workspaces\GitHub\TestR\$module\bin\$Configuration\*"
    New-Item $modulePath -ItemType Directory | Out-Null
    Copy-Item $sourcePath $modulePath\ -Recurse -Force
}

Write-Host
Set-Location $scriptPath
Write-Host "TestR Build: " $watch.Elapsed
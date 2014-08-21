$watch = [System.Diagnostics.Stopwatch]::StartNew()
$scriptPath = Split-Path (Get-Variable MyInvocation).Value.MyCommand.Path 
Set-Location $scriptPath
$destination = "C:\Binaries\TestR"
$nugetDestination = "C:\Workspaces\GitHub\Nuget"

if ([IO.Directory]::Exists($destination)) {
	[IO.Directory]::Delete($destination, $true)
}

$configuration = "Release"
if ($args.Count -gt 0) {
	$configuration = $args
}

$build = [Math]::Floor([DateTime]::UtcNow.Subtract([DateTime]::Parse("01/01/2000").Date).TotalDays)
$revision = [Math]::Floor([DateTime]::UtcNow.TimeOfDay.TotalSeconds / 2)

.\IncrementVersion.ps1 TestR\TestR $build $revision
.\IncrementVersion.ps1 TestR\TestR.PowerShell $build $revision
.\IncrementVersion.ps1 TestR\TestR.PowerShell.Tests $build $revision

$msbuild = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe"
cmd /c $msbuild "$scriptPath\TestR\TestR.sln" /p:Configuration="$configuration" /p:Platform="Any CPU" /t:Rebuild /p:VisualStudioVersion=12.0 /Verbosity:minimal /m
Set-Location $scriptPath

if (![System.IO.Directory]::Exists($destination)){
    [System.IO.Directory]::CreateDirectory(($destination))
}

xcopy "TestR\TestR\bin\$configuration\TestR.dll" $destination
xcopy "TestR\TestR\bin\$configuration\Interop.SHDocVw.dll" $destination

$versionInfo = [System.Diagnostics.FileVersionInfo]::GetVersionInfo("$destination\TestR.dll")
$version = $versionInfo.FileVersion.ToString()

cmd /c "TestR\.nuget\NuGet.exe" pack TestR.nuspec -Prop Configuration="$configuration" -Version $version
Move-Item "TestR.$version.nupkg" "$destination" -force
Copy-Item "$destination\TestR.$version.nupkg" "$nugetDestination" -force

.\ResetAssemblyInfos.ps1

$modulesPath = "C:\Workspaces\Harris\Deployment\Testing.Scripts\Modules"
if (Test-Path $modulesPath -PathType Container) {
    if (Test-Path $modulesPath\TestR.PowerShell -PathType Container) {
        Remove-Item $modulesPath\TestR.PowerShell -Force -Recurse
        
    }

    New-Item $modulesPath\TestR.PowerShell -ItemType Directory | Out-Null
    Copy-Item C:\Workspaces\GitHub\TestR\TestR\TestR.PowerShell\bin\$configuration\*  C:\Workspaces\Harris\Deployment\Testing.Scripts\Modules\TestR.PowerShell\ -Recurse -Force

    if (Test-Path $modulesPath\TestR.PowerShell.Tests -PathType Container) {
        Remove-Item $modulesPath\TestR.PowerShell.Tests -Force -Recurse
    }

    New-Item $modulesPath\TestR.PowerShell.Tests -ItemType Directory | Out-Null
    Copy-Item C:\Workspaces\GitHub\TestR\TestR\TestR.PowerShell.Tests\bin\$configuration\* C:\Workspaces\Harris\Deployment\Testing.Scripts\Modules\TestR.PowerShell.Tests\ -Recurse -Force
}

Write-Host
Set-Location $scriptPath
Write-Host "TestR Deploy: " $watch.Elapsed
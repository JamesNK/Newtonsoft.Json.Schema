properties { 
  $zipFileName = "JsonSchema20r3.zip"
  $majorVersion = "2.0"
  $majorWithReleaseVersion = "2.0.3"
  $nugetPrerelease = "beta1"
  $version = GetVersion $majorWithReleaseVersion
  $packageId = "Newtonsoft.Json.Schema"
  $signAssemblies = $false
  $signKeyPath = "C:\Development\Releases\newtonsoft.snk"
  $buildDocumentation = $false
  $buildNuGet = $true
  $treatWarningsAsErrors = $false
  $workingName = if ($workingName) {$workingName} else {"Working"}
  $netCliVersion = "1.0.0-preview1-002702"
  
  $baseDir  = resolve-path ..
  $buildDir = "$baseDir\Build"
  $sourceDir = "$baseDir\Src"
  $toolsDir = "$baseDir\Tools"
  $docDir = "$baseDir\Doc"
  $releaseDir = "$baseDir\Release"
  $workingDir = "$baseDir\$workingName"
  $workingSourceDir = "$workingDir\Src"
  $builds = @(
    @{Name = "Newtonsoft.Json.Schema.Dotnet"; TestsName = "Newtonsoft.Json.Schema.Tests.Dotnet"; BuildFunction = "NetCliBuild"; TestsFunction = "NetCliTests"; Constants="dotnet"; FinalDir="netstandard1.0"; NuGetDir = "netstandard1.0"; Framework=$null}
    @{Name = "Newtonsoft.Json.Schema"; TestsName = "Newtonsoft.Json.Schema.Tests"; BuildFunction = "MSBuildBuild"; TestsFunction = "NUnitTests"; Constants=$null; FinalDir="Net45"; NuGetDir = "net45"; Framework="net-4.0"; Sign=$true},
    @{Name = "Newtonsoft.Json.Schema.Net35"; TestsName = "Newtonsoft.Json.Schema.Net35.Tests"; BuildFunction = "MSBuildBuild"; TestsFunction = "NUnitTests"; Constants="NET35"; FinalDir="Net35"; NuGetDir = "net35"; Framework="net-2.0"; Sign=$true},
    @{Name = "Newtonsoft.Json.Schema.Net40"; TestsName = "Newtonsoft.Json.Schema.Net40.Tests"; BuildFunction = "MSBuildBuild"; TestsFunction = "NUnitTests"; Constants="NET40"; FinalDir="Net40"; NuGetDir = "net40"; Framework="net-4.0"; Sign=$true},
    @{Name = "Newtonsoft.Json.Schema.Portable"; TestsName = "Newtonsoft.Json.Schema.Tests.Portable"; BuildFunction = "MSBuildBuild"; TestsFunction = "NUnitTests"; Constants="PORTABLE"; FinalDir="Portable"; NuGetDir = "portable-net45+wp80+win8+wpa81"; Framework="net-4.0"; Sign=$true}
  )
}

framework '4.6x86'

task default -depends Test

# Ensure a clean working directory
task Clean {
  Set-Location $baseDir
  
  if (Test-Path -path $workingDir)
  {
    Write-Output "Deleting Working Directory"
    
    del $workingDir -Recurse -Force
  }
  
  New-Item -Path $workingDir -ItemType Directory
}

# Build each solution, optionally signed
task Build -depends Clean {
  Write-Host "Copying source to working source directory $workingSourceDir"
  robocopy $sourceDir $workingSourceDir /MIR /NP /XD bin obj TestResults AppPackages .vs artifacts DTAR_08E86330_4835_4B5C_9E5A_61F37AE1A077_DTAR /XF *.suo *.user *.lock.json | Out-Default

  Write-Host -ForegroundColor Green "Updating assembly version"
  Write-Host
  Update-AssemblyInfoFiles $workingSourceDir ($majorVersion + '.0.0') $version

  Write-Host -ForegroundColor Green "Signed $signAssemblies"
  if ($signAssemblies)
  {
    $files = Get-ChildItem -Path $workingSourceDir -Include @("project.json", "*.project.json") -Recurse
    $count = $files.Count

    Write-Host "Found $count files to update"
    foreach ($build in $files)
    {
      $fullName = $build.FullName
      Write-Host "Updating $fullName to use signed assembly"

      $content = gc $fullName
      $content = $content -replace 'Newtonsoft.Json.Unsigned','Newtonsoft.Json'
      sc $fullName $content
    }
  }

  Update-Project $workingSourceDir\Newtonsoft.Json.Schema\project.json $signAssemblies $true $true

  foreach ($build in $builds)
  {
    $name = $build.Name
    if ($name -ne $null)
    {
      Write-Host -ForegroundColor Green "Building " $name
      Write-Host -ForegroundColor Green "Signed " $signAssemblies
      Write-Host -ForegroundColor Green "Key " $signKeyPath

      & $build.BuildFunction $build
    }
  }
}

# Optional build documentation, add files to final zip
task Package -depends Build {
  New-Item -Path $workingDir\Package -ItemType Directory

  foreach ($build in $builds)
  {
    $name = $build.TestsName
    $finalDir = $build.FinalDir
    
    robocopy "$workingSourceDir\Newtonsoft.Json.Schema\bin\Release\$finalDir" $workingDir\Package\Bin\$finalDir Newtonsoft.Json.Schema.dll Newtonsoft.Json.Schema.pdb Newtonsoft.Json.Schema.xml /NP /XO /XF *.CodeAnalysisLog.xml | Out-Default
  }
  
  if ($buildNuGet)
  {
    $nugetVersion = $majorWithReleaseVersion
    if ($nugetPrerelease -ne $null)
    {
      $nugetVersion = $nugetVersion + "-" + $nugetPrerelease
    }

    New-Item -Path $workingDir\NuGet -ItemType Directory

    $nuspecPath = "$workingDir\NuGet\Newtonsoft.Json.Schema.nuspec"
    Copy-Item -Path "$buildDir\Newtonsoft.Json.Schema.nuspec" -Destination $nuspecPath -recurse

    Write-Host "Updating nuspec file at $nuspecPath" -ForegroundColor Green
    Write-Host

    $xml = [xml](Get-Content $nuspecPath)
    Edit-XmlNodes -doc $xml -xpath "//*[local-name() = 'id']" -value $packageId
    Edit-XmlNodes -doc $xml -xpath "//*[local-name() = 'version']" -value $nugetVersion

    Write-Host $xml.OuterXml

    $xml.save($nuspecPath)
    
    foreach ($build in $builds)
    {
      if ($build.NuGetDir)
      {
        $name = $build.TestsName
        $finalDir = $build.FinalDir
        $frameworkDirs = $build.NuGetDir.Split(",")
        
        foreach ($frameworkDir in $frameworkDirs)
        {
          robocopy "$workingSourceDir\Newtonsoft.Json.Schema\bin\Release\$finalDir" $workingDir\NuGet\lib\$frameworkDir Newtonsoft.Json.Schema.dll Newtonsoft.Json.Schema.pdb Newtonsoft.Json.Schema.xml /NP /XO /XF *.CodeAnalysisLog.xml | Out-Default
        }
      }
    }
  
    robocopy $workingSourceDir $workingDir\NuGet\src *.cs /S /NP /XD Newtonsoft.Json.Schema.Tests Newtonsoft.Json.Schema.TestConsole obj | Out-Default

    Write-Host "Building NuGet package with ID $packageId and version $nugetVersion" -ForegroundColor Green
    Write-Host

    exec { .\Tools\NuGet\NuGet.exe pack $workingDir\NuGet\Newtonsoft.Json.Schema.nuspec -Symbols }
    move -Path .\*.nupkg -Destination $workingDir\NuGet
  }
  
  if ($buildDocumentation)
  {
    $mainBuild = $builds | where { $_.Name -eq "Newtonsoft.Json.Schema" } | select -first 1
    $mainBuildFinalDir = $mainBuild.FinalDir
    $documentationSourcePath = "$workingSourceDir\Newtonsoft.Json.Schema.Tests\bin\Release\$mainBuildFinalDir"
    $docOutputPath = "$workingDir\Documentation\"
    Write-Host -ForegroundColor Green "Building documentation from $documentationSourcePath"
    Write-Host "Documentation output to $docOutputPath"

    # Sandcastle has issues when compiling with .NET 4 MSBuild - http://shfb.codeplex.com/Thread/View.aspx?ThreadId=50652
    exec { msbuild "/t:Clean;Rebuild" /p:Configuration=Release "/p:DocumentationSourcePath=$documentationSourcePath" "/p:OutputPath=$docOutputPath" $docDir\doc.shfbproj | Out-Default } "Error building documentation. Check that you have Sandcastle, Sandcastle Help File Builder and HTML Help Workshop installed."
    
    move -Path $workingDir\Documentation\LastBuild.log -Destination $workingDir\Documentation.log
  }

  Copy-Item -Path $docDir\readme.txt -Destination $workingDir\Package\
  Copy-Item -Path $docDir\license.txt -Destination $workingDir\Package\

  robocopy $workingSourceDir $workingDir\Package\Source\Src /MIR /NP /XD bin obj TestResults AppPackages DTAR_08E86330_4835_4B5C_9E5A_61F37AE1A077_DTAR /XF *.suo *.user | Out-Default
  robocopy $buildDir $workingDir\Package\Source\Build /MIR /NP /XF runbuild.txt | Out-Default
  robocopy $docDir $workingDir\Package\Source\Doc /MIR /NP | Out-Default
  robocopy $toolsDir $workingDir\Package\Source\Tools /MIR /NP | Out-Default
  
  exec { .\Tools\7-zip\7za.exe a -tzip $workingDir\$zipFileName $workingDir\Package\* | Out-Default } "Error zipping"
}

# Unzip package to a location
task Deploy -depends Package {
  exec { .\Tools\7-zip\7za.exe x -y "-o$workingDir\Deployed" $workingDir\$zipFileName | Out-Default } "Error unzipping"
}

# Run tests on deployed files
task Test -depends Deploy {
  foreach ($build in $builds)
  {
    if ($build.TestsFunction -ne $null)
    {
      & $build.TestsFunction $build
    }
  }
}

function MSBuildBuild($build)
{
  $name = $build.Name
  $finalDir = $build.FinalDir

  Write-Host
  Write-Host "Restoring $workingSourceDir\$name.sln" -ForegroundColor Green
  [Environment]::SetEnvironmentVariable("EnableNuGetPackageRestore", "true", "Process")
  exec { .\Tools\NuGet\NuGet.exe update -self }
  exec { .\Tools\NuGet\NuGet.exe restore "$workingSourceDir\$name.sln" -verbosity detailed -configfile $workingSourceDir\nuget.config | Out-Default } "Error restoring $name"

  $constants = GetConstants $build.Constants $signAssemblies

  Write-Host
  Write-Host "Building $workingSourceDir\$name.sln" -ForegroundColor Green
  Write-Host "Constants: $constants" -ForegroundColor Green
  exec { msbuild "/t:Clean;Rebuild" /p:Configuration=Release "/p:CopyNuGetImplementations=true" "/p:Platform=Any CPU" "/p:PlatformTarget=AnyCPU" /p:OutputPath=bin\Release\$finalDir\ /p:AssemblyOriginatorKeyFile=$signKeyPath "/p:SignAssembly=$signAssemblies" "/p:TreatWarningsAsErrors=$treatWarningsAsErrors" "/p:VisualStudioVersion=14.0" /p:DefineConstants=`"$constants`" "$workingSourceDir\$name.sln" | Out-Default } "Error building $name"
}

function NetCliBuild($build)
{
  $name = $build.Name
  $projectPath = "$workingSourceDir\Newtonsoft.Json.Schema\project.json"

  exec { .\Tools\Dotnet\install.ps1 -Version $netCliVersion | Out-Default }
  exec { dotnet --version | Out-Default }

  Write-Host -ForegroundColor Green "Restoring packages for $name"
  Write-Host
  exec { dotnet restore $projectPath | Out-Default }

  Write-Host -ForegroundColor Green "Building $projectPath"
  exec { dotnet build $projectPath -f netstandard1.3 -c Release -o bin\Release\netstandard1.3 | Out-Default }
}

function NetCliTests($build)
{
  if ($signAssemblies -ne $true) {
    Write-Host -ForegroundColor Yellow "Skipping .NET CLI tests because not signed build. Json.NET and Json.NET Unsigned can't co-exist"
    return
  }

  $name = $build.TestsName

  Update-Project $workingSourceDir\Newtonsoft.Json.Schema.Tests\project.json $signAssemblies $false $false

  exec { .\Tools\Dotnet\install.ps1 -Version $netCliVersion | Out-Default }
  exec { dotnet --version | Out-Default }

  Write-Host -ForegroundColor Green "Restoring packages for $name"
  Write-Host
  exec { dotnet restore "$workingSourceDir\Newtonsoft.Json.Schema.Tests\project.json" | Out-Default }

  Write-Host -ForegroundColor Green "Ensuring test project builds for $name"
  Write-Host

  try
  {
    Set-Location "$workingSourceDir\Newtonsoft.Json.Schema.Tests"
    exec { dotnet test "$workingSourceDir\Newtonsoft.Json.Schema.Tests\project.json" -f netcoreapp1.0 -c Release -parallel none | Out-Default }
  }
  finally
  {
    Set-Location $baseDir
  }
}

function NUnitTests($build)
{
  $name = $build.TestsName
  $finalDir = $build.FinalDir
  $framework = $build.Framework

  Write-Host -ForegroundColor Green "Copying test assembly $name to deployed directory"
  Write-Host
  robocopy "$workingSourceDir\Newtonsoft.Json.Schema.Tests\bin\Release\$finalDir" $workingDir\Deployed\Bin\$finalDir /MIR /NFL /NDL /NJS /NC /NS /NP /XO | Out-Default

  Copy-Item -Path "$workingSourceDir\Newtonsoft.Json.Schema.Tests\bin\Release\$finalDir\Newtonsoft.Json.Schema.Tests.dll" -Destination $workingDir\Deployed\Bin\$finalDir\

  Write-Host -ForegroundColor Green "Running NUnit tests " $name
  Write-Host
  exec { .\Tools\NUnit\nunit-console.exe "$workingDir\Deployed\Bin\$finalDir\Newtonsoft.Json.Schema.Tests.dll" /framework=$framework /xml:$workingDir\$name.xml | Out-Default } "Error running $name tests"
}

function GetNuGetVersion()
{
  $nugetVersion = $majorWithReleaseVersion
  if ($nugetPrerelease -ne $null)
  {
    $nugetVersion = $nugetVersion + "-" + $nugetPrerelease
  }

  return $nugetVersion
}

function GetConstants($constants, $includeSigned)
{
  $signed = switch($includeSigned) { $true { ";SIGNED" } default { "" } }

  return "CODE_ANALYSIS;TRACE;$constants$signed"
}

function GetVersion($majorVersion)
{
    $now = [DateTime]::Now
    
    $year = $now.Year - 2000
    $month = $now.Month
    $totalMonthsSince2000 = ($year * 12) + $month
    $day = $now.Day
    $minor = "{0}{1:00}" -f $totalMonthsSince2000, $day
    
    $hour = $now.Hour
    $minute = $now.Minute
    $revision = "{0:00}{1:00}" -f $hour, $minute
    
    return $majorVersion + "." + $minor
}

function Update-AssemblyInfoFiles ([string] $targetDir, [string] $assemblyVersionNumber, [string] $fileVersionNumber)
{
    $assemblyVersionPattern = 'AssemblyVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
    $fileVersionPattern = 'AssemblyFileVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
    $assemblyVersion = 'AssemblyVersion("' + $assemblyVersionNumber + '")';
    $fileVersion = 'AssemblyFileVersion("' + $fileVersionNumber + '")';
    
    Get-ChildItem -Path $targetDir -r -filter AssemblyInfo.cs | ForEach-Object {
        
        $filename = $_.Directory.ToString() + '\' + $_.Name
        Write-Host $filename
        $filename + ' -> ' + $version
    
        (Get-Content $filename) | ForEach-Object {
            % {$_ -replace $assemblyVersionPattern, $assemblyVersion } |
            % {$_ -replace $fileVersionPattern, $fileVersion }
        } | Set-Content $filename
    }
}

function Edit-XmlNodes {
    param (
        [xml] $doc,
        [string] $xpath = $(throw "xpath is a required parameter"),
        [string] $value = $(throw "value is a required parameter")
    )

    $nodes = $doc.SelectNodes($xpath)
    
    foreach ($node in $nodes) {
        if ($node -ne $null) {
            if ($node.NodeType -eq "Element") {
                $node.InnerXml = $value
            }
            else {
                $node.Value = $value
            }
        }
    }
}

function Update-Project {
  param (
    [string] $projectPath,
    [string] $sign,
    [bool] $xmlDoc,
    [bool] $warningsAsErrors
  )

  $file = switch($sign) { $true { $signKeyPath } default { $null } }

  $json = (Get-Content $projectPath) -join "`n" | ConvertFrom-Json
  $options = $json.buildOptions
  Add-Member -InputObject $options -MemberType NoteProperty -Name "warningsAsErrors" -Value $warningsAsErrors -Force
  Add-Member -InputObject $options -MemberType NoteProperty -Name "xmlDoc" -Value $xmlDoc -Force
  Add-Member -InputObject $options -MemberType NoteProperty -Name "keyFile" -Value $file -Force
  Add-Member -InputObject $options -MemberType NoteProperty -Name "define" -Value ((GetConstants "dotnet" $sign) -split ";") -Force

  $json.version = GetNuGetVersion

  ConvertTo-Json $json -Depth 10 | Set-Content $projectPath
}
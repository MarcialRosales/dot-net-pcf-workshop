Param (
    [string] $mode,
	[string] $targetFramework
)

# These are the settings that could be messed with, but shouldn't be.
Set-Variable -Option Constant -Name DefaultFramework     -Value "4.0.0.0"
Set-Variable -Option Constant -Name DefaultFrameworkPath -Value "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\"

Set-Variable -Option Constant -Name MsBuildApp           -Value "$DefaultFrameworkPath\MSBuild.exe"
Set-Variable -Option Constant -Name XUnitVersion         -Value 2.1.0
Set-Variable -Option Constant -Name XUnitApp             -Value "..\packages\xunit.runner.console.$XUnitVersion\tools\xunit.console"
Set-Variable -Option Constant -Name NugetPath            -Value ../nuget.exe
Set-Variable -Option Constant -Name PublisherVersion     -Value 14.0.0.3
#Set-Variable -Option Constant -Name PublisherPath        -Value "'C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\Microsoft\VisualStudio\v15.0'"
Set-Variable -Option Constant -Name PublisherPath        -Value "..\packages\Web.targets.$PublisherVersion\tools"


Set-Variable -Option Constant -Name Architecture         -Value x64
#Set-Variable -Option Constant -Name PublishProfile       -Value CF

#Set-Variable -Option Constant -Name MsBuildApp           -Value "'C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe'"

# Write a nice banner for the peoples.
function Write-Banner {
    . {
        Clear-Host
        Write-Host "

     ______       _ _     _  ______
     | ___ \     (_) |   | | | ___ \
     | |_/ /_   _ _| | __| | | |_/ / __ ___   ___ ___  ___ ___
     | ___ \ | | | | |/ _`` | |  __/ '__/ _ \ / __/ _ \/ __/ __|
     | |_/ / |_| | | | (_| | | |  | | | (_) | (_|  __/\__ \__ \
     \____/ \__,_|_|_|\__,_| \_|  |_|  \___/ \___\___||___/___/`n" -ForegroundColor Gray

        Write-Host "
               .--------.
              / .------. \
             / /        \ \
             | |        | |
            _| |________| |_
          .' |_|        |_| '.
          '._____ ____ _____.'
          |     .'____'.     |
          '.__.'.'    '.'.__.'
          '.__  | LDAP |  __.'
          |   '.'.____.'.'   |
          '.____'.____.'____.'
          '.________________.'" -ForegroundColor Green

		# Detect the .net frameworks on the build server.
		Write-Host "Detecting .Net Versions" -ForegroundColor Gray
		Detect-Frameworks

        # Show the parameters
        Write-Host "
        Mode:    $(Get-Mode)
        Machine: $((Get-WmiObject -Class Win32_ComputerSystem).Name)
        Target:  $(Get-TargetFramework)`n" -ForegroundColor Gray

		Write-Host "`nPerforming Install...`n" -ForegroundColor Gray

    } | Out-Null
}

# Get the current mode
function Get-Mode {
    . {
        if ($mode -eq '') {
            $mode = 'build'
        }

    } | Out-Null

    return $mode
}

# Holds the known frameworks
$knownFrameworks = @{}

# Detect the frameworks.
function Detect-Frameworks {
	. {
		if($script:knownFrameworks.Count -eq 0) {
			$frameworks = Get-ChildItem 'HKLM:\SOFTWARE\Microsoft\NET Framework Setup\NDP' -recurse |
				Get-ItemProperty -name Version,Release,InstallPath -EA 0 |
				Where { $_.PSChildName -match '^(?!S)\p{L}'} |
				Select PSChildName, Version, Release, InstallPath, @{
					name="Product"
					expression={
						switch -regex ($_.Release) {
						"378389" { [Version]"4.5" }
						"378675|378758" { [Version]"4.5.1" }
						"379893" { [Version]"4.5.2" }
						"393295|393297" { [Version]"4.6" }
						"394254|394271" { [Version]"4.6.1" }
						"394802|394806" { [Version]"4.6.2" }
						{$_ -gt 394806} { [Version]"Undocumented 4.6.2 or higher, please update script" }
						}
					}
				}

			foreach($framework in $frameworks) {
				if(-not [string]::IsNullOrEmpty($framework.Product) -and -not [string]::IsNullOrEmpty($framework.InstallPath)) {
					Write-Host "Framework: $($framework.Product) ($($framework.Version))" -ForegroundColor Magenta
					Write-Host "`tInstallPath: $($framework.InstallPath)" -ForegroundColor Magenta
                    $script:knownFrameworks.Set_Item($framework.Product.ToString(), $framework.InstallPath)

				    if(-not [string]::IsNullOrEmpty($framework.PSChildName)) {
					    Write-Host "`tProfile: $($framework.PSChildName)" -ForegroundColor Magenta
				    }
				    if(-not [string]::IsNullOrEmpty($framework.Release)) {
					    Write-Host "`tRelease: $($framework.Release)" -ForegroundColor Magenta
				    }
				} elseif($framework.Version -eq $DefaultFramework) {
					Write-Host "Legacy Framework: $($framework.Version)" -ForegroundColor Magenta
					Write-Host "`tInstallPath: $($DefaultFrameworkPath)" -ForegroundColor Magenta
                    $script:knownFrameworks.Set_Item($DefaultFramework, $DefaultFrameworkPath)
				    if(-not [string]::IsNullOrEmpty($framework.Release)) {
					    Write-Host "`tRelease: $($framework.Release)" -ForegroundColor Magenta
				    }

                } else {
					Write-Host "Unsupported Framework: $($framework.Version)" -ForegroundColor Yellow
				}
			}
		}
	} | Out-Null

	return $script:knownFrameworks
}

# Get the target framework version and the installation location.
function Get-TargetFramework {
    . {
        if($targetFramework -eq '') {
            $framework = $DefaultFramework
        } else {
            $framework = $targetFramework
        }

        $frameworkPath = $script:knownFrameworks.Get_Item($framework)
        Write-Host "Targeted Framework [$($framework)]: $($frameworkPath)" -ForegroundColor Gray

    } | Out-Null

	return $framework, $frameworkPath
}

function Install-NuGet() {
    . {
	    $uri = "dist.nuget.org/win-x86-commandline/latest/nuget.exe"
        Write-Host "Installing Nuget.exe" -ForegroundColor Green
            Invoke-WebRequest -Uri $uri  -Outfile nuget.exe | Write-Host
        Write-Host "nuget.exe installed successfully" -ForegroundColor Green
    } | Out-Null

}

# Handles the installation of NuGet packages
function NuGet-Install($package, $version) {
    . {
        Write-Host "Installing Nuget package $package, $version" -ForegroundColor Green
            Invoke-Expression "$NugetPath install $package -outputdirectory ..\packages -version $version" | Write-Host
        Write-Host "Package installed successfully" -ForegroundColor Green
    } | Out-Null
}

function NuGet-Restore() {
    . {
        Write-Host "Restoring Nuget package dependencies" -ForegroundColor Green
            Invoke-Expression "$NugetPath restore /PackagesDirectory  ..\packages " | Write-Host
        Write-Host "Packages installed successfully" -ForegroundColor Green
    } | Out-Null
}

function Configure-Publish {
    . {

        <#
            Always download this package and set up the override for the
            VSToolsPath as there may be dependencies in the MSBuild file on a
            specific version on VisualStudio
        #>
        Write-Host "Configuring publish support." -ForegroundColor Green
        NuGet-Install 'MSBuild.Microsoft.VisualStudio.Web.targets' $PublisherVersion
        $script:tools = "/p`:VSToolsPath=$PublisherPath"

        if($(Get-Mode) -eq 'publish') {
            $pre = '/p:DeployOnBuild=true`;PublishProfile='
            $script:publish = "$pre$PublishProfile"
            Write-Host $publish -ForegroundColor Yellow
        }
        Write-Host $tools -ForegroundColor Yellow
        Write-Host "Publish support configured." -ForegroundColor Green
    } | Out-Null
}



# Build the solution, return
function Build-Solution($configuration) {

    $code = -1
    . {


        Configure-Publish
        NuGet-Restore

        $frameworkVer, $frameworkPath = Get-TargetFramework
		$frameworkParam = ""
        if($frameworkVer -ne $DefaultFramework) {
            $frameworkParam = "/p`:FrameworkPathOverride=`"$frameworkPath`""
        }

        Write-Host "Listing folder ..\packages" -ForegroundColor Green
        Invoke-Expression "dir ..\packages\MSBuild.Microsoft.VisualStudio.Web.targets.14.0.0.3\tools\VSToolsPath" | Write-Host

        $app = "& $MsBuildApp /m /v:normal /p:Platform=$Architecture /p:Configuration=$configuration /nr:false $publish $tools $frameworkParam"

        Write-Host "Running the build script: $app" -ForegroundColor Green
        Invoke-Expression "$app" | Write-Host
        $code = $LastExitCode
    } | Out-Null

    if($code -ne 0) {
        Write-Host "Build FAILED." -ForegroundColor Red
    }
    else{
        Write-Host "Build SUCCESS." -ForegroundColor Green
    }

    $code
}

# The main entry point for this application.
function main {
    Write-Banner
#    Install-NuGet

    $buildConfig = if ($(Get-Mode) -eq 'test') {'Test'} Else {'Release'}
    $buildResult = Build-Solution $buildConfig

    if($buildResult -ne 0) {
        Write-Host "Build failed, aborting..." -ForegroundColor Red
        Exit $buildResult
    }

    if($(Get-Mode) -eq 'test') {
        Write-Host "Starting unit test execution" -ForegroundColor Green
        NuGet-Install 'xunit.runner.console' $XUnitVersion

        $failedUnitTests = 0

        #Get the matching test assemblies, ensure only bin and the target architecture are selected
        $testfiles = Get-ChildItem . -recurse  | where {$_.BaseName.EndsWith("Tests") -and $_.Extension -eq ".dll" `
            -and $_.FullName -match "\\bin\\" -and $_.FullName -match "$Architecture"  }

        #Execute unit tests in all assemblies, continue even in case of error, as it provides more context
        foreach($UnitTestDll in $testfiles) {
            Write-Host "Found Test: $($UnitTestDll.FullName)" -ForegroundColor Yellow
            Invoke-Expression "$XUnitApp $($UnitTestDll.FullName)"

            if( $LastExitCode -ne 0){
                $failedUnitTests++
                Write-Host "One or more tests in assembly FAILED" -ForegroundColor Red
            }
            else
            {
                Write-Host "All tests in assembly passed" -ForegroundColor Green
            }
        }

        if($failedUnitTests -ne 0) {
            Write-Host "Unit testing for $(Get-Mode) configuration FAILED." -ForegroundColor Red

        } else {
            Write-Host "Unit testing for $(Get-Mode) configuration completed successfully." -ForegroundColor Green
        }
        Exit $failedUnitTests
    }

}

main # Run the application.
#Install-NuGet

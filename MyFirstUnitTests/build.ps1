Param (
    [string] $mode
)

# These are the settings that could be messed with, but shouldn't be.
Set-Variable -Option Constant -Name DefaultFrameworkPath -Value "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\"
Set-Variable -Option Constant -Name MsBuildApp           -Value "$DefaultFrameworkPath\MSBuild.exe"

Set-Variable -Option Constant -Name XUnitVersion         -Value 2.3.1
Set-Variable -Option Constant -Name XUnitApp             -Value ".\packages\xunit.runner.console.$XUnitVersion\tools\net452\xunit.console.exe"
Set-Variable -Option Constant -Name NugetPath            -Value .\.nuget\nuget.exe
#Set-Variable -Option Constant -Name Architecture         -Value "'Any CPU'"
Set-Variable -Option Constant -Name Architecture         -Value x64

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


		 Write-Host "
          Mode:    $(Get-Mode)`n" -ForegroundColor Gray

    } | Out-Null

    Detect-Frameworks
}


# Detect the frameworks.
function Detect-Frameworks {
	Get-ChildItem 'HKLM:\SOFTWARE\Microsoft\NET Framework Setup\NDP' -recurse |
				Get-ItemProperty -name Version,Release,InstallPath -EA 0

}

# Handles the installation of NuGet packages
function NuGet-Install($package, $version) {
    . {
        Write-Host "Installing Nuget package $package, $version" -ForegroundColor Green
            Invoke-Expression "$NugetPath install $package -outputdirectory .\packages -version $version" | Write-Host
        Write-Host "Package installed successfully" -ForegroundColor Green
    } | Out-Null
}

function NuGet-Restore() {
    . {
        Write-Host "Restoring Nuget package dependencies" -ForegroundColor Green
            Invoke-Expression "$NugetPath restore -PackagesDirectory .\packages " | Write-Host
        Write-Host "Packages installed successfully" -ForegroundColor Green
    } | Out-Null
}


# Build the solution, return
function Build-Solution($configuration) {

    $code = -1
    . {
        NuGet-Restore

        Write-Host "Running the build script with configuration: $configuration"

        $app = "$MsBuildApp /m /v:normal /p:Configuration=$configuration /p:Platform=`"Any CPU`" /nr:false "

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

# Get the current mode
function Get-Mode {
    . {
        if ($mode -eq '') {
            $mode = 'build'
        }

    } | Out-Null

    return $mode
}

# The main entry point for this application.
function main {
    Write-Banner

    $buildConfig = if ($(Get-Mode) -eq 'test') {'Debug'} Else {'Release'}
    $buildResult = Build-Solution $buildConfig

    if($buildResult -ne 0) {
        Write-Host "Build failed, aborting..." -ForegroundColor Red
        Exit $buildResult
    }


    if($(Get-Mode) -eq 'test') {
        Write-Host "Starting unit test execution" -ForegroundColor Green

        $failedUnitTests = 0

        #Get the matching test assemblies, ensure only bin and the target architecture are selected
        $testfiles = Get-ChildItem . -recurse  | where {$_.BaseName.EndsWith("Tests") -and $_.Extension -eq ".dll" `
            -and $_.FullName -match "\\bin\\"  }

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

<#
.SYNOPSIS
Uses the information in the file CurrentVersion.xml to synthesize a file containing
compilation constants used to set the version attributes of the assembly being built.
#>

param(
    [Parameter(Mandatory=$true)] $outputDirectory,
    [Parameter(Mandatory=$true)] $namespace
)

$major, $minor, $patch, $preRelease = & "$PSScriptRoot\Get-VersionConstants.ps1"

$versionConstantsFileContents =
@"
// Licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System.Reflection;

[assembly: AssemblyVersion(${namespace}.VersionConstants.AssemblyVersion)]
[assembly: AssemblyFileVersion(${namespace}.VersionConstants.FileVersion)]

namespace $namespace
{
    public static class VersionConstants
    {
        public const string PreRelease = "$preRelease";
        public const string AssemblyVersion = "$major.$minor.$patch";
        public const string FileVersion = AssemblyVersion + ".0";
        public const string Version = AssemblyVersion + PreRelease;
    }
}
"@

$outputFile = "$outputDirectory\VersionConstants.cs"

Set-Content $outputFile $versionConstantsFileContents

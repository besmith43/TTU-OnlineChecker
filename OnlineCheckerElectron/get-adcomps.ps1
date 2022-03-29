param (
[System.Management.Automation.PSCredential]$Credential = $(Get-Credential),
[string]$CollegeOU
)

import-module ActiveDirectory

if (!$CollegeOU)
{
	$OU = "OU=CoA&S Wks,OU=TTU Workstations,DC=tntech,DC=edu"
	#$OU = "OU=English,OU=CoA&S Wks,OU=TTU Workstations,DC=tntech,DC=edu"
}
else
{
    $OU = $CollegeOU
}

$computers = get-adcomputer -filter * -searchbase $OU -property *

if (!(test-path -Path "$PSScriptRoot\AD-List.txt"))
{
    new-item -path "$PSScriptRoot\AD-List.txt" -ItemType "file"
}

foreach ($computer in $computers)
{
    $computer.name | out-file -filepath "$PSScriptRoot\AD-List.txt" -append
}
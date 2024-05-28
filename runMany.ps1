# Define the API endpoint
$apiUrl = 'https://localhost:52355/api/v1/GateWayControllerV1/proxy'

# Define the headers
$headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
$headers.Add("Content-Type", "application/json")
$headers.Add("Authorization", "Bearer ")

# Define the body of the request
$body = @"
`"teststring`"
"@

# Start 10 jobs in parallel
$jobs = @()
for ($i = 1; $i -le 10; $i++) {
    $jobs += Start-Job -ScriptBlock {
        param ($Url, $Headers, $Body)
        
        function Send-Request {
            param (
                [string]$Url,
                [hashtable]$Headers,
                [string]$Body
            )
            try {
                $response = Invoke-RestMethod -Uri $Url -Method Post -Headers $Headers -Body $Body
                return $response
            } catch {
                return $_.Exception.Message
            }
        }

        Send-Request -Url $Url -Headers $Headers -Body $Body
    } -ArgumentList $apiUrl, $headers, $body
}

# Wait for all jobs to complete
$jobs | Wait-Job

# Collect and display results
$jobs | ForEach-Object {
    $result = Receive-Job -Job $_
    Write-Host "Response from job $($_.Id): $($result | ConvertTo-Json)"
    Remove-Job -Job $_
}
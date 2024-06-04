# Define the API endpoint
$apiUrl = 'https://localhost:60684/api/v1/GateWayControllerV1/proxy'

# Define the headers
$headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
$headers.Add("Content-Type", "application/json")
$headers.Add("Authorization", "Bearer eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJfMl9LR0hfR0dHeWdTLVc2YzdOWHp2Sm5iUk5OcEhRS09YakdPRzdEUkJJIn0.eyJleHAiOjE3MTc1MDUxMDYsImlhdCI6MTcxNzUwNDgwNiwianRpIjoiYjhmYmVlZTUtZmE4Ni00NjIwLTliNGYtZjZmOTAwZTllMzE3IiwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo4MDgwL3JlYWxtcy90ZXN0cmVhbG0iLCJhdWQiOiJhY2NvdW50Iiwic3ViIjoiYWY3NzQ3NGQtZGZlMC00ZThjLTg3NzQtMTE1YjU3ZjAzNGViIiwidHlwIjoiQmVhcmVyIiwiYXpwIjoidGVzdGNsaWVudCIsImFjciI6IjEiLCJhbGxvd2VkLW9yaWdpbnMiOlsiaHR0cDovL2xvY2FsaG9zdDo4MDgwIl0sInJlYWxtX2FjY2VzcyI6eyJyb2xlcyI6WyJvZmZsaW5lX2FjY2VzcyIsInVtYV9hdXRob3JpemF0aW9uIiwiZGVmYXVsdC1yb2xlcy10ZXN0cmVhbG0iXX0sInJlc291cmNlX2FjY2VzcyI6eyJhY2NvdW50Ijp7InJvbGVzIjpbIm1hbmFnZS1hY2NvdW50IiwibWFuYWdlLWFjY291bnQtbGlua3MiLCJ2aWV3LXByb2ZpbGUiXX19LCJzY29wZSI6ImVtYWlsIHByb2ZpbGUiLCJjbGllbnRIb3N0IjoiMTcyLjE4LjAuMSIsImVtYWlsX3ZlcmlmaWVkIjpmYWxzZSwicHJlZmVycmVkX3VzZXJuYW1lIjoic2VydmljZS1hY2NvdW50LXRlc3RjbGllbnQiLCJjbGllbnRBZGRyZXNzIjoiMTcyLjE4LjAuMSIsImNsaWVudF9pZCI6InRlc3RjbGllbnQifQ.QZP4DTQGGA0C6j5DijbtqKoe7o9oNToRzVZ7dOG5i0bcuXRwr5WsFmWSEE65qOr6afGlMVmnxlXfCu3Zi5tC7o_ZRVCPhhyYiytULed__M4PVT3HB4GsO0LwH5o48tXmmsEz0-DBM3RFew7HqCGmrDNuPZSUj-Fp9jHFzD-D3jA9sg9QL_q8n3UH_pGpXAVP5IHPOcWap0ShxdlHeHygzXjYbCFm2UFTv_2kZdH4WKtqhexWCx2Ja9PMGAfmGT-9ZNUzkUiuPM0OD15hNHrKAcIMEUxaimXl_0SPPM5vIM_7faRRywSZHxVjFBIcz6akHnggUJYrTmVnsIGrQ8v3VA")

# Define the body of the request
$body = @"
`"teststring`"
"@

# Start 10 jobs in parallel
$jobs = @()
for ($i = 1; $i -le 30; $i++) {
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
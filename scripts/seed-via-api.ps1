# SmartScheduler - Data Seeding Script via API
# This PowerShell script seeds the database with synthetic data using the REST API

$baseUrl = "http://localhost:5062/api"

Write-Host "🌱 SmartScheduler Data Seeding Script" -ForegroundColor Cyan
Write-Host "======================================`n" -ForegroundColor Cyan

# Helper function to make API calls
function Invoke-ApiPost {
    param(
        [string]$endpoint,
        [hashtable]$body
    )

    $url = "$baseUrl/$endpoint"
    try {
        $response = Invoke-RestMethod -Uri $url -Method Post -Body ($body | ConvertTo-Json) -ContentType "application/json"
        Write-Host "✓ Created: $endpoint" -ForegroundColor Green
        return $response
    } catch {
        Write-Host "✗ Failed: $endpoint - $($_.Exception.Message)" -ForegroundColor Red
        return $null
    }
}

# Calculate dates
$tomorrow = (Get-Date).AddDays(1).ToString("yyyy-MM-dd")
$nextWeek = (Get-Date).AddDays(7).ToString("yyyy-MM-dd")

Write-Host "Step 1: Creating Contractors...`n" -ForegroundColor Yellow

# Contractor 1: Downtown Plumber
$contractor1 = Invoke-ApiPost "contractors" @{
    name = "Mike's Plumbing Pro"
    jobTypeId = 1
    rating = 4.8
    baseAddress = "123 Main St, Downtown"
    baseLatitude = 41.8781
    baseLongitude = -87.6298
    email = "mike@plumbingpro.com"
    phone = "555-0101"
    isActive = $true
    weeklySchedule = @(
        @{ dayOfWeek = 1; startTime = "08:00:00"; endTime = "17:00:00" },
        @{ dayOfWeek = 2; startTime = "08:00:00"; endTime = "17:00:00" },
        @{ dayOfWeek = 3; startTime = "08:00:00"; endTime = "17:00:00" },
        @{ dayOfWeek = 4; startTime = "08:00:00"; endTime = "17:00:00" },
        @{ dayOfWeek = 5; startTime = "08:00:00"; endTime = "17:00:00" }
    )
}

# Contractor 2: North Side Electrician
$contractor2 = Invoke-ApiPost "contractors" @{
    name = "Bright Spark Electric"
    jobTypeId = 2
    rating = 4.5
    baseAddress = "456 Oak Ave, North Side"
    baseLatitude = 41.9742
    baseLongitude = -87.6589
    email = "contact@brightspark.com"
    phone = "555-0102"
    isActive = $true
    weeklySchedule = @(
        @{ dayOfWeek = 1; startTime = "09:00:00"; endTime = "18:00:00" },
        @{ dayOfWeek = 2; startTime = "09:00:00"; endTime = "18:00:00" },
        @{ dayOfWeek = 3; startTime = "09:00:00"; endTime = "18:00:00" },
        @{ dayOfWeek = 4; startTime = "09:00:00"; endTime = "18:00:00" },
        @{ dayOfWeek = 5; startTime = "09:00:00"; endTime = "18:00:00" }
    )
}

# Contractor 3: West Side HVAC
$contractor3 = Invoke-ApiPost "contractors" @{
    name = "Climate Control Systems"
    jobTypeId = 3
    rating = 3.9
    baseAddress = "789 Elm St, West Side"
    baseLatitude = 41.8819
    baseLongitude = -87.7479
    email = "info@climatecontrol.com"
    phone = "555-0103"
    isActive = $true
    weeklySchedule = @(
        @{ dayOfWeek = 1; startTime = "07:00:00"; endTime = "16:00:00" },
        @{ dayOfWeek = 2; startTime = "07:00:00"; endTime = "16:00:00" },
        @{ dayOfWeek = 3; startTime = "07:00:00"; endTime = "16:00:00" },
        @{ dayOfWeek = 4; startTime = "07:00:00"; endTime = "16:00:00" },
        @{ dayOfWeek = 5; startTime = "07:00:00"; endTime = "16:00:00" },
        @{ dayOfWeek = 6; startTime = "07:00:00"; endTime = "16:00:00" }
    )
}

# Contractor 4: South Side Carpenter
$contractor4 = Invoke-ApiPost "contractors" @{
    name = "Precision Carpentry"
    jobTypeId = 4
    rating = 4.9
    baseAddress = "321 Pine Rd, South Side"
    baseLatitude = 41.7500
    baseLongitude = -87.6250
    email = "hello@precisionwood.com"
    phone = "555-0104"
    isActive = $true
    weeklySchedule = @(
        @{ dayOfWeek = 2; startTime = "08:00:00"; endTime = "17:00:00" },
        @{ dayOfWeek = 3; startTime = "08:00:00"; endTime = "17:00:00" },
        @{ dayOfWeek = 4; startTime = "08:00:00"; endTime = "17:00:00" },
        @{ dayOfWeek = 5; startTime = "08:00:00"; endTime = "17:00:00" },
        @{ dayOfWeek = 6; startTime = "08:00:00"; endTime = "17:00:00" }
    )
}

# Contractor 5: East Side Painter
$contractor5 = Invoke-ApiPost "contractors" @{
    name = "ColorPerfect Painting"
    jobTypeId = 5
    rating = 4.6
    baseAddress = "654 Maple Dr, East Side"
    baseLatitude = 41.8781
    baseLongitude = -87.6050
    email = "paint@colorperfect.com"
    phone = "555-0105"
    isActive = $true
    weeklySchedule = @(
        @{ dayOfWeek = 1; startTime = "07:00:00"; endTime = "15:00:00" },
        @{ dayOfWeek = 2; startTime = "07:00:00"; endTime = "15:00:00" },
        @{ dayOfWeek = 3; startTime = "07:00:00"; endTime = "15:00:00" },
        @{ dayOfWeek = 4; startTime = "07:00:00"; endTime = "15:00:00" },
        @{ dayOfWeek = 5; startTime = "07:00:00"; endTime = "15:00:00" }
    )
}

# Contractor 6: Another Plumber (lower rating)
$contractor6 = Invoke-ApiPost "contractors" @{
    name = "Quick Fix Plumbing"
    jobTypeId = 1
    rating = 3.5
    baseAddress = "999 West Blvd, Far West"
    baseLatitude = 41.8781
    baseLongitude = -87.8000
    email = "service@quickfix.com"
    phone = "555-0106"
    isActive = $true
    weeklySchedule = @(
        @{ dayOfWeek = 1; startTime = "10:00:00"; endTime = "19:00:00" },
        @{ dayOfWeek = 2; startTime = "10:00:00"; endTime = "19:00:00" },
        @{ dayOfWeek = 3; startTime = "10:00:00"; endTime = "19:00:00" },
        @{ dayOfWeek = 4; startTime = "10:00:00"; endTime = "19:00:00" },
        @{ dayOfWeek = 5; startTime = "10:00:00"; endTime = "19:00:00" }
    )
}

# Contractor 7: Premium Electrician
$contractor7 = Invoke-ApiPost "contractors" @{
    name = "Elite Electrical Services"
    jobTypeId = 2
    rating = 5.0
    baseAddress = "100 Center St, Downtown"
    baseLatitude = 41.8789
    baseLongitude = -87.6300
    email = "elite@electric.com"
    phone = "555-0107"
    isActive = $true
    weeklySchedule = @(
        @{ dayOfWeek = 1; startTime = "08:00:00"; endTime = "18:00:00" },
        @{ dayOfWeek = 2; startTime = "08:00:00"; endTime = "18:00:00" },
        @{ dayOfWeek = 3; startTime = "08:00:00"; endTime = "18:00:00" },
        @{ dayOfWeek = 4; startTime = "08:00:00"; endTime = "18:00:00" },
        @{ dayOfWeek = 5; startTime = "08:00:00"; endTime = "18:00:00" }
    )
}

Write-Host "`nStep 2: Creating Jobs...`n" -ForegroundColor Yellow

# Job 1: Plumbing job downtown (should match Mike's Plumbing Pro)
Invoke-ApiPost "jobs" @{
    jobTypeId = 1
    customerId = "CUST-001"
    customerName = "Acme Corporation"
    desiredDate = $tomorrow
    desiredTime = "10:00:00"
    locationAddress = "200 Main St, Downtown"
    locationLatitude = 41.8785
    locationLongitude = -87.6295
    estimatedDurationHours = 2.0
}

# Job 2: Electrical job north side (should match Bright Spark or Elite)
Invoke-ApiPost "jobs" @{
    jobTypeId = 2
    customerId = "CUST-002"
    customerName = "TechStart Inc"
    desiredDate = $tomorrow
    desiredTime = "14:00:00"
    locationAddress = "500 Oak Ave, North Side"
    locationLatitude = 41.9750
    locationLongitude = -87.6600
    estimatedDurationHours = 3.0
}

# Job 3: HVAC job far west (should match Climate Control, but distance penalty)
Invoke-ApiPost "jobs" @{
    jobTypeId = 3
    customerId = "CUST-003"
    customerName = "West Mall Properties"
    desiredDate = $nextWeek
    desiredTime = "09:00:00"
    locationAddress = "1000 West Blvd, Far West"
    locationLatitude = 41.8800
    locationLongitude = -87.7800
    estimatedDurationHours = 4.0
}

# Job 4: Carpentry job south (perfect for Precision Carpentry)
Invoke-ApiPost "jobs" @{
    jobTypeId = 4
    customerId = "CUST-004"
    customerName = "Home Remodel Co"
    desiredDate = $tomorrow
    desiredTime = "11:00:00"
    locationAddress = "350 Pine Rd, South Side"
    locationLatitude = 41.7520
    locationLongitude = -87.6240
    estimatedDurationHours = 5.0
}

# Job 5: Painting job east (ColorPerfect match)
Invoke-ApiPost "jobs" @{
    jobTypeId = 5
    customerId = "CUST-005"
    customerName = "Luxury Apartments LLC"
    desiredDate = $tomorrow
    desiredTime = "08:00:00"
    locationAddress = "700 Maple Dr, East Side"
    locationLatitude = 41.8790
    locationLongitude = -87.6045
    estimatedDurationHours = 6.0
}

Write-Host "`n✅ Seeding complete!`n" -ForegroundColor Green
Write-Host "Summary:" -ForegroundColor Cyan
Write-Host "  - 7 Contractors created with weekly schedules" -ForegroundColor White
Write-Host "  - 5 Jobs created (all unassigned for testing recommendations)" -ForegroundColor White
Write-Host "`nRefresh your browser to see the new data!" -ForegroundColor Yellow

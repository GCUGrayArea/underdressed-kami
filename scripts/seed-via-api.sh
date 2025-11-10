#!/bin/bash
# SmartScheduler - Seed data via REST API

BASE_URL="http://localhost:5062/api"

echo "🌱 SmartScheduler Data Seeding Script"
echo "======================================"
echo ""

# Job Type IDs (from database)
PLUMBING_ID="05d30bd9-1198-456f-bf42-fa76463af1e1"
ELECTRICAL_ID="f7e4b094-468d-4007-93c9-5eee37e592ec"
HVAC_ID="61801784-368b-4e8e-8329-3092e16e2af8"
CARPENTRY_ID="a4247a3a-8208-42fb-b02d-8f31ba8ee7f6"
PAINTING_ID="4cb09b30-facd-407a-a34f-e93f292d1c87"

echo "Step 1: Creating Contractors..."
echo ""

# Contractor 1: Downtown Plumber
curl -s -X POST "$BASE_URL/contractors" \
  -H "Content-Type: application/json" \
  -d '{
    "Name": "Mikes Plumbing Pro",
    "JobTypeId": "'$PLUMBING_ID'",
    "Rating": 4.8,
    "BaseLocation": {
      "Latitude": 41.8781,
      "Longitude": -87.6298,
      "Address": "123 Main St, Downtown"
    },
    "Email": "mike@plumbingpro.com",
    "Phone": "555-0101",
    "WeeklySchedule": [
      {"DayOfWeek": 1, "StartTime": "08:00:00", "EndTime": "17:00:00"},
      {"DayOfWeek": 2, "StartTime": "08:00:00", "EndTime": "17:00:00"},
      {"DayOfWeek": 3, "StartTime": "08:00:00", "EndTime": "17:00:00"},
      {"DayOfWeek": 4, "StartTime": "08:00:00", "EndTime": "17:00:00"},
      {"DayOfWeek": 5, "StartTime": "08:00:00", "EndTime": "17:00:00"}
    ]
  }' > /dev/null && echo "✓ Created: Mike's Plumbing Pro"

# Contractor 2: North Side Electrician
curl -s -X POST "$BASE_URL/contractors" \
  -H "Content-Type: application/json" \
  -d '{
    "Name": "Bright Spark Electric",
    "JobTypeId": "'$ELECTRICAL_ID'",
    "Rating": 4.5,
    "BaseLocation": {
      "Latitude": 41.9742,
      "Longitude": -87.6589,
      "Address": "456 Oak Ave, North Side"
    },
    "Email": "contact@brightspark.com",
    "Phone": "555-0102",
    "WeeklySchedule": [
      {"DayOfWeek": 1, "StartTime": "09:00:00", "EndTime": "18:00:00"},
      {"DayOfWeek": 2, "StartTime": "09:00:00", "EndTime": "18:00:00"},
      {"DayOfWeek": 3, "StartTime": "09:00:00", "EndTime": "18:00:00"},
      {"DayOfWeek": 4, "StartTime": "09:00:00", "EndTime": "18:00:00"},
      {"DayOfWeek": 5, "StartTime": "09:00:00", "EndTime": "18:00:00"}
    ]
  }' > /dev/null && echo "✓ Created: Bright Spark Electric"

# Contractor 3: West Side HVAC
curl -s -X POST "$BASE_URL/contractors" \
  -H "Content-Type: application/json" \
  -d '{
    "Name": "Climate Control Systems",
    "JobTypeId": "'$HVAC_ID'",
    "Rating": 3.9,
    "BaseLocation": {
      "Latitude": 41.8819,
      "Longitude": -87.7479,
      "Address": "789 Elm St, West Side"
    },
    "Email": "info@climatecontrol.com",
    "Phone": "555-0103",
    "WeeklySchedule": [
      {"DayOfWeek": 1, "StartTime": "07:00:00", "EndTime": "16:00:00"},
      {"DayOfWeek": 2, "StartTime": "07:00:00", "EndTime": "16:00:00"},
      {"DayOfWeek": 3, "StartTime": "07:00:00", "EndTime": "16:00:00"},
      {"DayOfWeek": 4, "StartTime": "07:00:00", "EndTime": "16:00:00"},
      {"DayOfWeek": 5, "StartTime": "07:00:00", "EndTime": "16:00:00"},
      {"DayOfWeek": 6, "StartTime": "07:00:00", "EndTime": "16:00:00"}
    ]
  }' > /dev/null && echo "✓ Created: Climate Control Systems"

# Contractor 4: South Side Carpenter
curl -s -X POST "$BASE_URL/contractors" \
  -H "Content-Type: application/json" \
  -d '{
    "Name": "Precision Carpentry",
    "JobTypeId": "'$CARPENTRY_ID'",
    "Rating": 4.9,
    "BaseLocation": {
      "Latitude": 41.7500,
      "Longitude": -87.6250,
      "Address": "321 Pine Rd, South Side"
    },
    "Email": "hello@precisionwood.com",
    "Phone": "555-0104",
    "WeeklySchedule": [
      {"DayOfWeek": 2, "StartTime": "08:00:00", "EndTime": "17:00:00"},
      {"DayOfWeek": 3, "StartTime": "08:00:00", "EndTime": "17:00:00"},
      {"DayOfWeek": 4, "StartTime": "08:00:00", "EndTime": "17:00:00"},
      {"DayOfWeek": 5, "StartTime": "08:00:00", "EndTime": "17:00:00"},
      {"DayOfWeek": 6, "StartTime": "08:00:00", "EndTime": "17:00:00"}
    ]
  }' > /dev/null && echo "✓ Created: Precision Carpentry"

# Contractor 5: East Side Painter
curl -s -X POST "$BASE_URL/contractors" \
  -H "Content-Type: application/json" \
  -d '{
    "Name": "ColorPerfect Painting",
    "JobTypeId": "'$PAINTING_ID'",
    "Rating": 4.6,
    "BaseLocation": {
      "Latitude": 41.8781,
      "Longitude": -87.6050,
      "Address": "654 Maple Dr, East Side"
    },
    "Email": "paint@colorperfect.com",
    "Phone": "555-0105",
    "WeeklySchedule": [
      {"DayOfWeek": 1, "StartTime": "07:00:00", "EndTime": "15:00:00"},
      {"DayOfWeek": 2, "StartTime": "07:00:00", "EndTime": "15:00:00"},
      {"DayOfWeek": 3, "StartTime": "07:00:00", "EndTime": "15:00:00"},
      {"DayOfWeek": 4, "StartTime": "07:00:00", "EndTime": "15:00:00"},
      {"DayOfWeek": 5, "StartTime": "07:00:00", "EndTime": "15:00:00"}
    ]
  }' > /dev/null && echo "✓ Created: ColorPerfect Painting"

echo ""
echo "Step 2: Creating Jobs..."
echo ""

# Calculate tomorrow's date
TOMORROW=$(date -d "tomorrow" +%Y-%m-%d 2>/dev/null || date -v+1d +%Y-%m-%d)

# Job 1: Plumbing job downtown
curl -s -X POST "$BASE_URL/jobs" \
  -H "Content-Type: application/json" \
  -d '{
    "JobTypeId": "'$PLUMBING_ID'",
    "CustomerId": "CUST-001",
    "CustomerName": "Acme Corporation",
    "DesiredDate": "'$TOMORROW'",
    "DesiredTime": "10:00:00",
    "Location": {
      "Latitude": 41.8785,
      "Longitude": -87.6295,
      "Address": "200 Main St, Downtown"
    },
    "EstimatedDurationHours": 2.0
  }' > /dev/null && echo "✓ Created: Plumbing job - Acme Corporation"

# Job 2: Electrical job north side
curl -s -X POST "$BASE_URL/jobs" \
  -H "Content-Type: application/json" \
  -d '{
    "JobTypeId": "'$ELECTRICAL_ID'",
    "CustomerId": "CUST-002",
    "CustomerName": "TechStart Inc",
    "DesiredDate": "'$TOMORROW'",
    "DesiredTime": "14:00:00",
    "Location": {
      "Latitude": 41.9750,
      "Longitude": -87.6600,
      "Address": "500 Oak Ave, North Side"
    },
    "EstimatedDurationHours": 3.0
  }' > /dev/null && echo "✓ Created: Electrical job - TechStart Inc"

# Job 3: HVAC job far west
NEXT_WEEK=$(date -d "+7 days" +%Y-%m-%d 2>/dev/null || date -v+7d +%Y-%m-%d)
curl -s -X POST "$BASE_URL/jobs" \
  -H "Content-Type: application/json" \
  -d '{
    "JobTypeId": "'$HVAC_ID'",
    "CustomerId": "CUST-003",
    "CustomerName": "West Mall Properties",
    "DesiredDate": "'$NEXT_WEEK'",
    "DesiredTime": "09:00:00",
    "Location": {
      "Latitude": 41.8800,
      "Longitude": -87.7800,
      "Address": "1000 West Blvd, Far West"
    },
    "EstimatedDurationHours": 4.0
  }' > /dev/null && echo "✓ Created: HVAC job - West Mall Properties"

# Job 4: Carpentry job south
curl -s -X POST "$BASE_URL/jobs" \
  -H "Content-Type: application/json" \
  -d '{
    "JobTypeId": "'$CARPENTRY_ID'",
    "CustomerId": "CUST-004",
    "CustomerName": "Home Remodel Co",
    "DesiredDate": "'$TOMORROW'",
    "DesiredTime": "11:00:00",
    "Location": {
      "Latitude": 41.7520,
      "Longitude": -87.6240,
      "Address": "350 Pine Rd, South Side"
    },
    "EstimatedDurationHours": 5.0
  }' > /dev/null && echo "✓ Created: Carpentry job - Home Remodel Co"

# Job 5: Painting job east
curl -s -X POST "$BASE_URL/jobs" \
  -H "Content-Type: application/json" \
  -d '{
    "JobTypeId": "'$PAINTING_ID'",
    "CustomerId": "CUST-005",
    "CustomerName": "Luxury Apartments LLC",
    "DesiredDate": "'$TOMORROW'",
    "DesiredTime": "08:00:00",
    "Location": {
      "Latitude": 41.8790,
      "Longitude": -87.6045,
      "Address": "700 Maple Dr, East Side"
    },
    "EstimatedDurationHours": 6.0
  }' > /dev/null && echo "✓ Created: Painting job - Luxury Apartments LLC"

echo ""
echo "✅ Seeding complete!"
echo ""
echo "Summary:"
echo "  - 5 Contractors created with weekly schedules"
echo "  - 5 Jobs created (all unassigned for testing recommendations)"
echo ""
echo "Refresh your browser to see the new data!"

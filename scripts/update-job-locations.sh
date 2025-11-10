#!/bin/bash
# Update existing jobs with proper Chicago coordinates

BASE_URL="http://localhost:5062/api"

echo "🔧 Updating job locations with Chicago coordinates..."
echo ""

# Get all jobs
JOBS=$(curl -s "$BASE_URL/jobs")

# Extract job IDs and update each one
# Job 1: Downtown (near Mike's Plumbing)
curl -s -X PATCH "$BASE_URL/jobs" \
  -H "Content-Type: application/json" \
  -d '{
    "id": "'"$(echo $JOBS | jq -r '.[0].id')"'",
    "location": {
      "latitude": 41.8785,
      "longitude": -87.6295,
      "address": "200 Main St, Downtown"
    }
  }' > /dev/null && echo "✓ Updated Job 1 location"

# Job 2: North Side (near Bright Spark)
curl -s -X PATCH "$BASE_URL/jobs" \
  -H "Content-Type: application/json" \
  -d '{
    "id": "'"$(echo $JOBS | jq -r '.[1].id')"'",
    "location": {
      "latitude": 41.9750,
      "longitude": -87.6600,
      "address": "500 Oak Ave, North Side"
    }
  }' > /dev/null && echo "✓ Updated Job 2 location"

# Job 3: Far West (near Climate Control)
curl -s -X PATCH "$BASE_URL/jobs" \
  -H "Content-Type: application/json" \
  -d '{
    "id": "'"$(echo $JOBS | jq -r '.[2].id')"'",
    "location": {
      "latitude": 41.8800,
      "longitude": -87.7800,
      "address": "1000 West Blvd, Far West"
    }
  }' > /dev/null && echo "✓ Updated Job 3 location"

# Job 4: South Side (near Precision Carpentry)
curl -s -X PATCH "$BASE_URL/jobs" \
  -H "Content-Type: application/json" \
  -d '{
    "id": "'"$(echo $JOBS | jq -r '.[3].id')"'",
    "location": {
      "latitude": 41.7520,
      "longitude": -87.6240,
      "address": "350 Pine Rd, South Side"
    }
  }' > /dev/null && echo "✓ Updated Job 4 location"

# Job 5: East Side (near ColorPerfect)
curl -s -X PATCH "$BASE_URL/jobs" \
  -H "Content-Type: application/json" \
  -d '{
    "id": "'"$(echo $JOBS | jq -r '.[4].id')"'",
    "location": {
      "latitude": 41.8790,
      "longitude": -87.6045,
      "address": "700 Maple Dr, East Side"
    }
  }' > /dev/null && echo "✓ Updated Job 5 location"

echo ""
echo "✅ All job locations updated!"

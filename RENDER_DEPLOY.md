# Quick Render Deployment Guide

## Prerequisites
1. **OpenRouteService API Key**: Get from https://openrouteservice.org/dev/#/signup
2. **Render Account**: Sign up at https://render.com (free tier is fine)
3. **GitHub**: Code must be pushed to a GitHub repository

## Step-by-Step Deployment

### 1. Get OpenRouteService API Key
- Go to https://openrouteservice.org/dev/#/signup
- Sign up (free)
- Confirm your email
- Go to your dashboard and copy the API key
- Keep it handy for step 4

### 2. Push Code to GitHub
```bash
# If you haven't already pushed to GitHub:
git add render.yaml RENDER_DEPLOY.md
git commit -m "Add Render deployment configuration"
git push origin main
```

### 3. Deploy Using Render Blueprint

1. Go to https://dashboard.render.com
2. Click **"New"** → **"Blueprint"**
3. Connect your GitHub repository
4. Render will detect the `render.yaml` file
5. Click **"Apply"**

Render will automatically create:
- PostgreSQL database (smartscheduler-db)
- Backend web service (smartscheduler-backend)
- Frontend web service (smartscheduler-frontend)

### 4. Add OpenRouteService API Key

After deployment starts:

1. Go to **Dashboard** → **smartscheduler-backend**
2. Click **"Environment"** tab
3. Find `OPENROUTESERVICE_API_KEY`
4. Click **"Edit"**
5. Paste your API key
6. Click **"Save Changes"**

The backend will automatically redeploy with the API key.

### 5. Wait for Build to Complete

- Backend build: ~5-10 minutes (.NET 8 compilation)
- Frontend build: ~3-5 minutes (React + Vite)
- Database: ~2 minutes (PostgreSQL setup)

Monitor build logs in real-time on the Render dashboard.

### 6. Verify Deployment

Once all services show "Live" status:

**Backend Health Check:**
- Go to: `https://smartscheduler-backend.onrender.com/health`
- Should return: `{"status":"Healthy","timestamp":"..."}`

**Frontend:**
- Go to: `https://smartscheduler-frontend.onrender.com`
- Should load the SmartScheduler dashboard

### 7. Test the Application

1. Navigate to the frontend URL
2. Go to **Contractors** page
3. Click **"Add Contractor"**
4. Fill in contractor details
5. Save and verify it appears in the list
6. Go to **Jobs** page
7. Click **"Create Job"**
8. Fill in job details and save
9. Click on the job to see recommendations

## Troubleshooting

### Backend fails to start
**Check logs:** Dashboard → smartscheduler-backend → Logs

Common issues:
- Missing `OPENROUTESERVICE_API_KEY` → Add it in Environment tab
- Database connection failure → Check that database is "Live"

### Frontend shows blank page
**Check console errors in browser:**
- Backend not responding → Verify backend health endpoint
- CORS errors → Check backend CORS configuration in render.yaml

### Database migrations fail
**The backend automatically applies migrations on startup.**

To manually run migrations:
1. Go to smartscheduler-backend → Shell tab
2. Run: `dotnet ef database update`

### Slow first load (cold start)
Render's free tier spins down services after 15 minutes of inactivity. First request after idle takes 30-60 seconds to wake up. This is normal for free tier.

## Costs

**Free Tier (sufficient for demo):**
- PostgreSQL: Free 90 days, then $7/month
- Backend web service: Free (750 hours/month)
- Frontend web service: Free (750 hours/month)

**Paid Tier (recommended for production):**
- PostgreSQL: $7/month (Starter plan)
- Backend: $7/month (Starter plan, no cold starts)
- Frontend: $7/month (Starter plan, no cold starts)
- **Total: ~$21/month**

## URLs After Deployment

- **Frontend**: https://smartscheduler-frontend.onrender.com
- **Backend API**: https://smartscheduler-backend.onrender.com
- **API Health**: https://smartscheduler-backend.onrender.com/health
- **Swagger/OpenAPI**: https://smartscheduler-backend.onrender.com/swagger

## Next Steps

After successful deployment:
1. Test all features (contractors, jobs, recommendations)
2. Add sample data for demo
3. Share frontend URL with stakeholders
4. (Optional) Set up custom domain in Render settings

## Support

If deployment fails:
1. Check build logs in Render dashboard
2. Verify all environment variables are set
3. Check that Docker images build successfully locally
4. Review the troubleshooting section above

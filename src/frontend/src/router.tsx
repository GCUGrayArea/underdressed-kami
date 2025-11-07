import { lazy, Suspense } from 'react';
import { createBrowserRouter } from 'react-router-dom';
import { MainLayout } from './layouts/MainLayout';
import { LoadingFallback } from './components/LoadingFallback';

// Lazy load page components for code splitting
const Dashboard = lazy(() =>
  import('./pages/Dashboard').then((module) => ({ default: module.Dashboard }))
);

const Contractors = lazy(() =>
  import('./pages/Contractors').then((module) => ({ default: module.Contractors }))
);

const Jobs = lazy(() =>
  import('./pages/Jobs').then((module) => ({ default: module.Jobs }))
);

const NotFound = lazy(() =>
  import('./pages/NotFound').then((module) => ({ default: module.NotFound }))
);

/**
 * Wraps lazy-loaded components with Suspense boundary
 */
function withSuspense(Component: React.ComponentType) {
  return (
    <Suspense fallback={<LoadingFallback />}>
      <Component />
    </Suspense>
  );
}

/**
 * Application router configuration using React Router v7
 */
export const router = createBrowserRouter([
  {
    path: '/',
    element: <MainLayout />,
    children: [
      {
        index: true,
        element: withSuspense(Dashboard),
      },
      {
        path: 'contractors',
        element: withSuspense(Contractors),
      },
      {
        path: 'jobs',
        element: withSuspense(Jobs),
      },
      {
        path: '*',
        element: withSuspense(NotFound),
      },
    ],
  },
]);

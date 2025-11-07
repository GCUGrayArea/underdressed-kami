import { RouterProvider } from 'react-router-dom';
import { router } from './router';

/**
 * Root application component
 * Provides routing context to the application
 */
function App() {
  return <RouterProvider router={router} />;
}

export default App;

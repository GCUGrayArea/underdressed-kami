import { List, ListItem, ListItemButton, ListItemIcon, ListItemText } from '@mui/material';
import DashboardIcon from '@mui/icons-material/Dashboard';
import PeopleIcon from '@mui/icons-material/People';
import WorkIcon from '@mui/icons-material/Work';
import { useNavigate, useLocation } from 'react-router-dom';

interface NavItem {
  label: string;
  path: string;
  icon: React.ReactNode;
}

const navItems: NavItem[] = [
  { label: 'Dashboard', path: '/', icon: <DashboardIcon /> },
  { label: 'Contractors', path: '/contractors', icon: <PeopleIcon /> },
  { label: 'Jobs', path: '/jobs', icon: <WorkIcon /> },
];

/**
 * Navigation links component for main menu
 */
export function Navigation() {
  const navigate = useNavigate();
  const location = useLocation();

  return (
    <List>
      {navItems.map((item) => {
        const isActive = location.pathname === item.path;

        return (
          <ListItem key={item.path} disablePadding>
            <ListItemButton
              selected={isActive}
              onClick={() => navigate(item.path)}
            >
              <ListItemIcon sx={{ color: isActive ? 'primary.main' : 'inherit' }}>
                {item.icon}
              </ListItemIcon>
              <ListItemText primary={item.label} />
            </ListItemButton>
          </ListItem>
        );
      })}
    </List>
  );
}

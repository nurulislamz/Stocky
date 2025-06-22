import * as React from 'react';
import { useLocation, useMatch, useNavigate } from 'react-router-dom';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import Stack from '@mui/material/Stack';
import HomeRoundedIcon from '@mui/icons-material/HomeRounded';
import AnalyticsRoundedIcon from '@mui/icons-material/AnalyticsRounded';
import PeopleRoundedIcon from '@mui/icons-material/PeopleRounded';
import AssignmentRoundedIcon from '@mui/icons-material/AssignmentRounded';
import SettingsRoundedIcon from '@mui/icons-material/SettingsRounded';
import InfoRoundedIcon from '@mui/icons-material/InfoRounded';
import HelpRoundedIcon from '@mui/icons-material/HelpRounded';
import AccountBalanceWalletIcon from '@mui/icons-material/AccountBalanceWallet';
import CalculateIcon from '@mui/icons-material/Calculate';
import ScreenSearchDesktopIcon from '@mui/icons-material/ScreenSearchDesktop';
import NewspaperIcon from '@mui/icons-material/Newspaper';
import StarBorderRoundedIcon from '@mui/icons-material/StarBorderRounded';
import DashboardRoundedIcon from '@mui/icons-material/DashboardRounded';
import ShowChartRoundedIcon from '@mui/icons-material/ShowChartRounded';
import SearchRoundedIcon from '@mui/icons-material/SearchRounded';
import { Link } from 'react-router-dom';
import LogoutRoundedIcon from '@mui/icons-material/LogoutRounded';
import { useAuth } from '../hooks/useAuth';


const mainListItems = [
  { text: "Home", icon: <DashboardRoundedIcon />, to: "/home"  },
  { text: "Search", icon: <SearchRoundedIcon />, to: "/search"  },
  {
    text: "Portfolio",
    icon: <AccountBalanceWalletIcon />,
    to: "/portfolio"
  },
  {
    text: "Watchlist",
    icon: <StarBorderRoundedIcon />,
    to: "/watchlist"
  },
  {
    text: "Market Data",
    icon: <ShowChartRoundedIcon />,
    to: "/marketdata"
  },
  {
    text: "Stock Screener",
    icon: <ScreenSearchDesktopIcon />,
    to: "/stockscreener"
  },
  {
    text: "Leverage ETF Calculator",
    icon: <CalculateIcon />,
    to: "/etfcalculator"
  },
  {
    text: "News Feed",
    icon: <NewspaperIcon />,
    to: "/newsfeed"
  },
];

const secondaryListItems = [
  {
    text: "Settings",
    icon: <SettingsRoundedIcon />,
    to: "/settings"
  },
  { text: "About", icon: <InfoRoundedIcon />, to: "/about", itemNumber: 10 },
  {
    text: "Feedback",
    icon: <HelpRoundedIcon />,
    to: "/feedback"
  }
];

export default function MenuContent() {
  const currentLocation = useLocation();
  const navigate = useNavigate();
  const { logout } = useAuth();

  const handleLogout = () => {
    logout();
    navigate('/login');
    window.location.reload();
  };

  return (
    <Stack sx={{ flexGrow: 1, p: 1, justifyContent: "space-between" }}>
      <List dense>
        {mainListItems.map((item, index) => (
          <ListItem key={index} disablePadding sx={{ display: "block" }}>
            <ListItemButton
              selected={currentLocation.pathname === item.to}
              component={Link}
              to={item.to}
            >
              <ListItemIcon>{item.icon}</ListItemIcon>
              <ListItemText
                primary={item.text}
                sx={{
                  '& .MuiListItemText-primary': {
                    overflow: 'hidden',
                    textOverflow: 'ellipsis',
                    whiteSpace: 'nowrap',
                  }
                }}
              />
            </ListItemButton>
          </ListItem>
        ))}
      </List>
      <List dense>
        {secondaryListItems.map((item, index) => (
          <ListItem key={index} disablePadding sx={{ display: "block" }}>
            <ListItemButton component={Link} to={item.to}>
              <ListItemIcon>{item.icon}</ListItemIcon>
              <ListItemText
                primary={item.text}
                sx={{
                  '& .MuiListItemText-primary': {
                    overflow: 'hidden',
                    textOverflow: 'ellipsis',
                    whiteSpace: 'nowrap',
                  }
                }}
              />
            </ListItemButton>
          </ListItem>
        ))}
        <ListItem disablePadding sx={{ display: "block" }}>
          <ListItemButton onClick={handleLogout}>
            <ListItemIcon><LogoutRoundedIcon /></ListItemIcon>
            <ListItemText
              primary="Logout"
              sx={{
                '& .MuiListItemText-primary': {
                  overflow: 'hidden',
                  textOverflow: 'ellipsis',
                  whiteSpace: 'nowrap',
                }
              }}
            />
          </ListItemButton>
        </ListItem>
      </List>
    </Stack>
  );
}

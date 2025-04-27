import React, { useState } from 'react';
import Drawer from '@mui/material/Drawer';
import Button from '@mui/material/Button';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemText from '@mui/material/ListItemText';
import MenuIcon from "@mui/icons-material/Menu"
import HomeIcon from "@mui/icons-material/Home"
import AccountIcon from "@mui/icons-material/AccountCircle"
import SettingsIcon from "@mui/icons-material/Settings"
import LogoutIcon from "@mui/icons-material/Logout"


const SideMenu = () => {
  const [isOpen, setIsOpen] = useState<boolean>(false);

  return (
    <>
      <Button onClick={() => setIsOpen(true)}>
        <MenuIcon></MenuIcon>
      </Button>
      <Drawer anchor="left" open={isOpen} onClose={() => setIsOpen(false)}>
        <List>
          <ListItem>
            <ListItemText primary="Home"/>
          </ListItem>
          <ListItem>
            <ListItemText primary="Portfolio"/>
          </ListItem>
          <ListItem>
            <ListItemText primary="Leverage ETF Calculator"/>
          </ListItem>
          <ListItem>
            <ListItemText primary="News Feed"/>
          </ListItem>
          <ListItem>
            <ListItemText primary="Key indicators"/>
          </ListItem>
          <ListItem>
            <ListItemText primary="Stock Screener"/>
          </ListItem>
          <ListItem>
            <ListItemText primary="Settings"/>
          </ListItem>
          <ListItem>
            <ListItemText primary="Logout"/>
          </ListItem>
        </List>
      </Drawer>
    </>
  )
}

export default SideMenu;
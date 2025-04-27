import React, { ReactNode } from 'react';
import Box from "@mui/material/Box";
import Paper from '@mui/material/Paper'

type LoginLayoutProps = {
  children: ReactNode;
}

const LoginLayout = ({children}:LoginLayoutProps)  => { return (
    <Box
      sx={{
        minHeight: "100vh",
        bgcolor: "background.default",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
      }}
    >
      <Paper
        elevation={3}
        sx={{
          padding: 4,
          minWidth: 320,
          maxWidth: 400,
          width: "100%",
        }}
      >
        {children}
      </Paper>
    </Box>
  );
};

export default LoginLayout;
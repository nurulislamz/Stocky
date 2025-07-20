import Header from "../../components/Header";
import Layout from "../../templates/Layout";
import * as React from "react";
import Grid from "@mui/material/Grid";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import Copyright from "../../internals/components/Copyright";

export default function UserSettingsPage(props: { disableCustomTheme?: boolean }) {
  return (
    <Layout>
      <Header headerName="User Settings" />
      <Box sx={{ width: "100%", maxWidth: { sm: "100%", md: "1700px" } }}>
          <Typography
            component="h1"
            variant="h4"
            sx={{ width: '100%', fontSize: 'clamp(2rem, 10vw, 2.15rem)' }}
          >
            User Settings
          </Typography>
      </Box>
    </Layout>
  );
}

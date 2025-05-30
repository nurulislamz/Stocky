import Header from "../../components/Header";
import Layout from "../../templates/Layout";
import * as React from "react";
import Grid from "@mui/material/Grid";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import Copyright from "../../internals/components/Copyright";

export default function MarketDataPage(props: { disableCustomTheme?: boolean }) {
  return (
    <Layout>
      <Header headerName="Home" />
      <Box sx={{ width: "100%", maxWidth: { sm: "100%", md: "1700px" } }}>
        <Typography component="h2" variant="h6" sx={{ mb: 2 }}>
          Indicators
        </Typography>
        <Grid spacing={2} columns={12}>
          Move Index, Inverse Yield Curve, Etc
        </Grid>
        <Copyright sx={{ my: 4 }} />
      </Box>
    </Layout>
  );
}

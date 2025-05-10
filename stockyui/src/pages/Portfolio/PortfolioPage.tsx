import { useState, useEffect } from 'react';
import type {} from '@mui/x-charts/themeAugmentation';
import type {} from '@mui/x-data-grid-pro/themeAugmentation';
import type {} from '@mui/x-tree-view/themeAugmentation';
import Header from '../../components/Header';
import Box from '@mui/material/Box';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import Copyright from '../../internals/components/Copyright';
import CustomizedDataGrid from '../../components/CustomizedDataGrid';
import PortfolioPieChart from '../../components/PortfolioPieChart';
import PortfolioChart from "../../components/PortfolioChart";
import StatCard, { StatCardProps } from "../../components/StatCard";
import Layout from "../../templates/Layout";
import Grid from '@mui/material/Grid';

const data: StatCardProps[] = [
  {
    title: "Portfolio Value",
    value: "14k",
    interval: "Last 30 days",
    trend: "up",
    data: [
      200, 24, 220, 260, 240, 380, 100, 240, 280, 240, 300, 340, 320, 360, 340,
      380, 360, 400, 380, 420, 400, 640, 340, 460, 440, 480, 460, 600, 880, 920,
    ],
  }
];

export default function PortfolioPage(props: { disableCustomTheme?: boolean }) {
  const [portfolioValue, setPortfolioValue] = useState<number | null>(null);

  useEffect(() => {
    setPortfolioValue(100);
  }, []);

  return (
    <Layout>
      <Header headerName="Portfolio" />

      <Box sx={{ width: "100%", maxWidth: { sm: "100%", md: "1700px" } }}>
        <Typography component="h2" variant="h6" sx={{ mb: 2, fontWeight: 'bold' }} align="center">
          £{portfolioValue}
        </Typography>

      <Grid
        container
        spacing={2}
        columns={12}
        sx={{ mb: (theme) => theme.spacing(2) }}
      >
        <Grid
          size={{ xs: 12, lg: 9 }}
          sx={{
            height: {
              xs: "300px",
              md: "400px",
              lg: "500px",
            },
          }}
        >
          <PortfolioChart />
        </Grid>
        <Grid
          size={{ xs: 12, lg: 3 }}
          sx={{
            height: {
              xs: "300px",
              md: "400px",
              lg: "500px",
            },
          }}
        >
          <PortfolioPieChart />
        </Grid>
        </Grid>
        <Typography component="h2" variant="h6" sx={{ mb: 2 }}>
          Tickers
        </Typography>
        <Grid spacing={2} columns={12}>
          <CustomizedDataGrid />
        </Grid>
        <Copyright sx={{ my: 4 }} />
      </Box>
    </Layout>
  );
}


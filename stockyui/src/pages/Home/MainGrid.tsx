import * as React from 'react';
import Grid from '@mui/material/Grid';
import Box from '@mui/material/Box';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import Copyright from '../../internals/components/Copyright';
import PortfolioTable from '../../components/PortfolioTable';
import PortfolioPieChart from '../../components/PortfolioPieChart';
import PortfolioChart from "../../components/PortfolioChart";
import { StatCardProps } from "../../components/StatCard";
import StatCardGrid from "../../components/StatCardGrid";
import data from "../../data/data";

const Overview = ({ data }: { data: StatCardProps[] }) => {
  return (
    <>
      <Grid
        container
        spacing={2}
        columns={12}
        sx={{ mb: (theme) => theme.spacing(2) }}
      >
        <StatCardGrid data={data} />
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
    </>
  );
};

export default function MainGrid() {
  return (
    <Box sx={{ width: '100%', maxWidth: { sm: '100%', md: '1700px' } }}>
      <Typography component="h2" variant="h6" sx={{ mb: 2 }}>
        Overview
      </Typography>
      <Overview data={data} />
      <Typography component="h2" variant="h6" sx={{ mb: 2 }}>
        Upcoming News
      </Typography>
      <Grid spacing={2} columns={12}>
        <PortfolioTable />
      </Grid>
      <Copyright sx={{ my: 4 }} />
    </Box>
  );
}

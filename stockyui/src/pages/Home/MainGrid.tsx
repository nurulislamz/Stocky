import Grid from '@mui/material/Grid';
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import Copyright from '../../internals/components/Copyright';
import PortfolioTable from '../../components/PortfolioTable';
import PortfolioPieChart from '../../components/PortfolioPieChart';
import PortfolioChart from "../../components/PortfolioChart";
import StatCardGrid from "../../components/StatCardGrid";
import { usePortfolio } from '../../contexts/PortfolioContext';
import { StatCardProps } from '../../components/StatCard';

export default function MainGrid() {
  const { data, refresh } = usePortfolio();

  // Transform portfolio data into StatCardProps array
  const statCards: StatCardProps[] = data ? [
    {
      title: "Portfolio Value",
      value: `${data?.totalValue?.toLocaleString()}`,
      interval: "Last 30 days",
      trend:  new Error("Not implemented") ? "up" : "down",
      data: [] // Assuming you have historical data
    },
    {
      title: "Cash Balance",
      value: `${data?.cashBalance?.toLocaleString()}`,
      interval: "Current",
      trend: "neutral",
      data: [] // Add historical cash balance data if available
    },
    {
      title: "Invested Amount",
      value: `${data?.investedAmount?.toLocaleString()}`,
      interval: "Current",
      trend: "neutral",
      data: [] // Add historical investment data if available
    }
  ] : [];

  return (
    <Box sx={{ width: '100%', maxWidth: { sm: '100%', md: '1700px' } }}>
      <Typography component="h2" variant="h6" sx={{ mb: 2 }}>
        Overview
      </Typography>
      <Grid
        container
        spacing={2}
        columns={12}
        sx={{ mb: (theme) => theme.spacing(2) }}
      >
        <StatCardGrid data={statCards} />
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
          <PortfolioPieChart data={null} totalValue="98.5K" totalLabel="Total" />
        </Grid>
      </Grid>
      <Typography component="h2" variant="h6" sx={{ mb: 2 }}>
        Portfolio Table
      </Typography>
      <Grid spacing={2} columns={12}>
        <PortfolioTable/>
      </Grid>
      <Copyright sx={{ my: 4 }} />
    </Box>
  );
}

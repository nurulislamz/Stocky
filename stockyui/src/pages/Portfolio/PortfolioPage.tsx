import { useState, useEffect } from 'react';
import type {} from '@mui/x-charts/themeAugmentation';
import type {} from '@mui/x-data-grid-pro/themeAugmentation';
import type {} from '@mui/x-tree-view/themeAugmentation';
import Header from '../../components/Header';
import Box from '@mui/material/Box';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import Button from '@mui/material/Button';
import AddIcon from '@mui/icons-material/Add';
import Divider from '@mui/material/Divider';
import Copyright from '../../internals/components/Copyright';
import PortfolioTable from '../../components/PortfolioTable';
import PortfolioPieChart from '../../components/PortfolioPieChart';
import PortfolioChart from "../../components/PortfolioChart";
import StatCard, { StatCardProps } from "../../components/StatCard";
import Layout from "../../templates/Layout";
import Grid from '@mui/material/Grid';
import BuyTradeModal from '../../components/BuyTradeModal';
import AddFundsModal from '../../components/AddFundsModal';
import { usePortfolio } from '../../hooks/usePortfolio';

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
  const [buyModalOpen, setBuyModalOpen] = useState(false);
  const [addFundsModalOpen, setAddFundsModalOpen] = useState(false);

  const {
    totalValue,
    cashBalance,
    investedAmount,
    isLoading,
    fetchPortfolio
  } = usePortfolio();

  const handleAddTickerClick = () => {
    setBuyModalOpen(true);
  };

  // Refresh portfolio when modals close
  const handleAddFundsClose = () => {
    setAddFundsModalOpen(false);
    fetchPortfolio(); // Refresh portfolio data
  };

  const handleBuyModalClose = () => {
    setBuyModalOpen(false);
    fetchPortfolio(); // Refresh portfolio data
  };

  return (
    <Layout>
      <Header headerName="Portfolio" />

      <Box sx={{ width: "100%", maxWidth: { sm: "100%", md: "1700px" } }}>
        <Box sx={{
          p: { xs: 2, sm: 3 },
          backgroundColor: 'background.paper',
          borderRadius: 2,
          boxShadow: '0 1px 3px rgba(0,0,0,0.1)',
          border: '1px solid',
          borderColor: 'divider',
          mb: 2
        }}>
          {/* Desktop Layout */}
          <Box sx={{
            display: { xs: 'none', sm: 'flex' },
            justifyContent: 'space-between',
            alignItems: 'center'
          }}>
            <Box sx={{ display: 'flex', gap: 4, alignItems: 'center' }}>
              <Box>
                <Typography variant="h4" sx={{ fontWeight: 700, color: 'text.primary', mb: 0.5 }}>
                  £{isLoading ? '...' : (totalValue || 0).toFixed(2)}
                </Typography>
                <Typography variant="body2" sx={{ color: 'text.secondary', fontSize: '0.875rem', lineHeight: 1.43 }}>
                  Total Portfolio Value
                </Typography>
              </Box>
              <Divider orientation="vertical" flexItem sx={{ height: '60px', mx: 2 }} />
              <Box>
                <Typography variant="h4" sx={{ fontWeight: 700, color: 'text.primary', mb: 0.5 }}>
                  £{isLoading ? '...' : (investedAmount || 0).toFixed(2)}
                </Typography>
                <Typography variant="body2" sx={{ color: 'text.secondary', fontSize: '0.875rem', lineHeight: 1.43 }}>
                  Invested Amount
                </Typography>
              </Box>
              <Divider orientation="vertical" flexItem sx={{ height: '60px', mx: 2 }} />
              <Box>
                <Typography variant="h4" sx={{ fontWeight: 700, color: 'text.primary', mb: 0.5 }}>
                  £{isLoading ? '...' : (cashBalance || 0).toFixed(2)}
                </Typography>
                <Typography variant="body2" sx={{ color: 'text.secondary', fontSize: '0.875rem', lineHeight: 1.43 }}>
                  Cash Balance
                </Typography>
              </Box>
            </Box>
            <Button
              variant="outlined"
              startIcon={<AddIcon />}
              onClick={() => {
                setAddFundsModalOpen(true);
              }}
              sx={{
                borderColor: 'primary.main',
                color: 'primary.main',
                borderRadius: 1.5,
                px: 2.5,
                py: 1,
                fontWeight: 500,
                textTransform: 'none',
                '&:hover': {
                  borderColor: 'primary.dark',
                  backgroundColor: 'primary.main',
                  color: 'white',
                },
              }}
            >
              Add Funds
            </Button>
          </Box>

          {/* Mobile Layout */}
          <Box sx={{ display: { xs: 'block', sm: 'none' } }}>
            <Stack spacing={2}>
              <Box sx={{ textAlign: 'center' }}>
                <Typography variant="h4" sx={{ fontWeight: 700, color: 'text.primary', mb: 0.5 }}>
                  £{isLoading ? '...' : (totalValue || 0).toFixed(2)}
                </Typography>
                <Typography variant="body2" sx={{ color: 'text.secondary', fontSize: '0.875rem', lineHeight: 1.43 }}>
                  Total Portfolio Value
                </Typography>
              </Box>

              <Divider sx={{ my: 1 }} />

              <Box sx={{ textAlign: 'center' }}>
                <Typography variant="h4" sx={{ fontWeight: 700, color: 'text.primary', mb: 0.5 }}>
                  £{isLoading ? '...' : (investedAmount || 0).toFixed(2)}
                </Typography>
                <Typography variant="body2" sx={{ color: 'text.secondary', fontSize: '0.875rem', lineHeight: 1.43 }}>
                  Invested Amount
                </Typography>
              </Box>

              <Divider sx={{ my: 1 }} />

              <Box sx={{ textAlign: 'center' }}>
                <Typography variant="h4" sx={{ fontWeight: 700, color: 'text.primary', mb: 0.5 }}>
                  £{isLoading ? '...' : (cashBalance || 0).toFixed(2)}
                </Typography>
                <Typography variant="body2" sx={{ color: 'text.secondary', fontSize: '0.875rem', lineHeight: 1.43 }}>
                  Cash Balance
                </Typography>
              </Box>

              <Button
                variant="outlined"
                startIcon={<AddIcon />}
                onClick={() => {
                  setAddFundsModalOpen(true);
                }}
                fullWidth
                sx={{
                  borderColor: 'primary.main',
                  color: 'primary.main',
                  borderRadius: 1.5,
                  py: 1.5,
                  fontWeight: 500,
                  textTransform: 'none',
                  fontSize: '1rem',
                  '&:hover': {
                    borderColor: 'primary.dark',
                    backgroundColor: 'primary.main',
                    color: 'white',
                  },
                }}
              >
                Add Funds
              </Button>
            </Stack>
          </Box>
        </Box>

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

        {/* Tickers Section */}
        <Box sx={{
          display: 'flex',
          flexDirection: { xs: 'column', sm: 'row' },
          justifyContent: 'space-between',
          alignItems: { xs: 'stretch', sm: 'center' },
          gap: { xs: 2, sm: 0 },
          mb: 2
        }}>
          <Typography component="h2" variant="h6" sx={{ textAlign: { xs: 'center', sm: 'left' } }}>
            Tickers
          </Typography>
          <Button
            variant="contained"
            startIcon={<AddIcon />}
            onClick={() => {
              handleAddTickerClick();
            }}
            sx={{
              backgroundColor: '#1976d2',
              borderRadius: 1.5,
              py: { xs: 1.5, sm: 1 },
              px: { xs: 2, sm: 2.5 },
              fontWeight: 500,
              textTransform: 'none',
              fontSize: { xs: '1rem', sm: '0.875rem' },
              '&:hover': {
                backgroundColor: '#1565c0',
              },
            }}
          >
            Add Ticker
          </Button>
        </Box>

        <PortfolioTable />
        <Copyright sx={{ my: 4 }} />
      </Box>

      <BuyTradeModal
        open={buyModalOpen}
        onClose={handleBuyModalClose}
        symbol=""
        price="$0.00"
      />

      <AddFundsModal
        open={addFundsModalOpen}
        onClose={handleAddFundsClose}
      />
    </Layout>
  );
}


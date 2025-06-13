import * as React from 'react';
import { PieChart } from '@mui/x-charts/PieChart';
import { useDrawingArea } from '@mui/x-charts/hooks';
import { styled } from '@mui/material/styles';
import Typography from '@mui/material/Typography';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Box from '@mui/material/Box';
import Stack from '@mui/material/Stack';
import LinearProgress, { linearProgressClasses } from '@mui/material/LinearProgress';

interface StyledTextProps {
  variant: 'primary' | 'secondary';
}

const StyledText = styled('text', {
  shouldForwardProp: (prop) => prop !== 'variant',
})<StyledTextProps>(({ theme }) => ({
  textAnchor: 'middle',
  dominantBaseline: 'central',
  fill: theme.palette.text.secondary,
  variants: [
    {
      props: {
        variant: 'primary',
      },
      style: {
        fontSize: theme.typography.h5.fontSize,
      },
    },
    {
      props: ({ variant }) => variant !== 'primary',
      style: {
        fontSize: theme.typography.body2.fontSize,
      },
    },
    {
      props: {
        variant: 'primary',
      },
      style: {
        fontWeight: theme.typography.h5.fontWeight,
      },
    },
    {
      props: ({ variant }) => variant !== 'primary',
      style: {
        fontWeight: theme.typography.body2.fontWeight,
      },
    },
  ],
}));

interface PieCenterLabelProps {
  primaryText: string;
  secondaryText: string;
}

function PieCenterLabel({ primaryText, secondaryText }: PieCenterLabelProps) {
  const { width, height, left, top } = useDrawingArea();
  const primaryY = top + height / 2 - 10;
  const secondaryY = primaryY + 24;

  return (
    <React.Fragment>
      <StyledText variant="primary" x={left + width / 2} y={primaryY}>
        {primaryText}
      </StyledText>
      <StyledText variant="secondary" x={left + width / 2} y={secondaryY}>
        {secondaryText}
      </StyledText>
    </React.Fragment>
  );
}

interface PortfolioPieChartProps {
  data: Array<{
    id: string;
    value: number;
    label: string;
    color: string;
  }> | null;
  totalValue: string;
  totalLabel: string;
}

export default function PortfolioPieChart({ data, totalValue, totalLabel }: PortfolioPieChartProps) {
  if (!data) {
    return (
      <Card
        variant="outlined"
        sx={{ display: 'flex', flexDirection: 'column', gap: '8px', flexGrow: 1, height: '100%', width: '100%'}}
      >
        <CardContent>
          <Typography component="h2" variant="subtitle2" sx={{ mb: 4 }}>
            Portfolio %
          </Typography>
          <Box sx={{
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            mt: 4,
            mb: 4,
            height: '250px'
          }}>
            <Typography variant="body1" color="text.secondary">
              No portfolio data available
            </Typography>
          </Box>
        </CardContent>
      </Card>
    );
  }

  return (
    <Card
      variant="outlined"
      sx={{ display: 'flex', flexDirection: 'column', gap: '8px', flexGrow: 1, height: '100%', width: '100%'}}
    >
      <CardContent>
        <Typography component="h2" variant="subtitle2" sx={{ mb: 4 }}>
          Portfolio %
        </Typography>
        <Box sx={{
          display: 'flex',
          alignItems: 'center',
          mt: 4,
          mb: 4
        }}>
          <PieChart
            sx={{
              height: {
                xs: '180px',
                sm: '220px',
                md: '250px'
              },
              width: '100%'
            }}
            series={[
              {
                data,
                innerRadius: 50,
                outerRadius: 70,
                paddingAngle: 0,
                highlightScope: { fade: 'global', highlight: 'item' },
              }
            ]}
            slotProps={{
              legend: { direction: "horizontal", position: { horizontal: 'center', vertical: 'bottom' } },
            }}
          >
            <PieCenterLabel primaryText={totalValue} secondaryText={totalLabel} />
          </PieChart>
        </Box>
        {data.map((item) => (
          <Stack
            key={item.id}
            direction="row"
            sx={{ alignItems: 'center', gap: 2, pb: 2 }}
          >
            <Stack sx={{ gap: 1, flexGrow: 1 }}>
              <Stack
                direction="row"
                sx={{
                  justifyContent: 'space-between',
                  alignItems: 'center',
                  gap: 2,
                }}
              >
                <Typography variant="body2" sx={{ fontWeight: '500' }}>
                  {item.label}
                </Typography>
                <Typography variant="body2" sx={{ color: 'text.secondary' }}>
                  {item.value}%
                </Typography>
              </Stack>
              <LinearProgress
                variant="determinate"
                value={item.value}
                sx={{
                  [`& .${linearProgressClasses.bar}`]: {
                    backgroundColor: item.color,
                  },
                }}
              />
            </Stack>
          </Stack>
        ))}
      </CardContent>
    </Card>
  );
}

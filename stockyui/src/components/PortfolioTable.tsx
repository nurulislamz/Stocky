import React, { useEffect } from 'react';
import { DataGrid, GridColDef } from '@mui/x-data-grid';
import { usePortfolio } from '../hooks/usePortfolio';
import { StockyApi } from '../services/generated/stockyapi';

const columns: GridColDef[] = [
  { field: 'symbol', headerName: 'Symbol', flex: 1, minWidth: 100, align: 'right', headerAlign: 'right' },
  { field: 'quantity', headerName: 'Shares', flex: 1, minWidth: 100, type: 'number', align: 'right', headerAlign: 'right' },
  {
    field: 'averageBuyPrice',
    headerName: 'Avg Price',
    flex: 1,
    minWidth: 120,
    align: 'right',
    headerAlign: 'right'
  },
  {
    field: 'currentPrice',
    headerName: 'Current Price',
    flex: 1,
    minWidth: 120,
    align: 'right',
    headerAlign: 'right'
  },
  {
    field: 'totalValue',
    headerName: 'Total Value',
    flex: 1,
    minWidth: 120,
    align: 'right',
    headerAlign: 'right'
  },
  {
    field: 'profitLoss',
    headerName: 'P/L',
    flex: 1,
    minWidth: 120,
    align: 'right',
    headerAlign: 'right'
  },
  {
    field: 'profitLossPercentage',
    headerName: 'P/L %',
    flex: 1,
    minWidth: 100,
    align: 'right',
    headerAlign: 'right'
  },
  {
    field: 'actions',
    headerName: 'Actions',
    flex: 1,
    minWidth: 120,
    sortable: false,
    align: 'right',
    headerAlign: 'right',
    renderCell: (params) => (
      <div style={{
        display: 'flex',
        gap: '8px',
        justifyContent: 'flex-end',
        width: '100%'
      }}>
        <button
          onClick={() => console.log('Edit clicked for id:', params.row.id)}
          style={{
            border: 'none',
            background: 'none',
            color: '#1976d2',
            cursor: 'pointer',
            padding: '4px',
            textAlign: 'right',
          }}
        >
          Edit
        </button>
        <button
          onClick={() => console.log('Delete clicked for id:', params.row.id)}
          style={{
            border: 'none',
            background: 'none',
            color: '#d32f2f',
            cursor: 'pointer',
            padding: '4px',
            textAlign: 'right',
          }}
        >
          Delete
        </button>
      </div>
    ),
  },
];

export default function PortfolioTable() {
  const { items, isLoading, error } = usePortfolio();

  const rows = React.useMemo(() => {
    if (!items || items.length === 0) {
      return [];
    }

    console.log(items);

    return items.map((item: StockyApi.PortfolioItem, index: number) => ({
      id: item.symbol || `item-${index}`,
      symbol: item.symbol || '',
      quantity: item.quantity || 0,
      averageBuyPrice: `£${item.averageBuyPrice || 0}`,
      currentPrice: `£${item.currentPrice || 0}`,
      totalValue: `£${item.totalValue || 0}`,
      profitLoss: `£${item.profitLoss || 0}`,
      profitLossPercentage: `${item.profitLossPercentage || 0}%`,
      lastUpdatedTime: item.lastUpdatedTime || ''
    }));
  }, [items]);

  if (isLoading) {
    return <div>Loading portfolio data...</div>;
  }

  if (error) {
    return <div>Error loading portfolio: {error}</div>;
  }

  return (
    <DataGrid
      checkboxSelection
      rows={rows}
      columns={columns}
      getRowClassName={(params) =>
        params.indexRelativeToCurrentPage % 2 === 0 ? 'even' : 'odd'
      }
      initialState={{
        pagination: { paginationModel: { pageSize: 20 } },
      }}
      pageSizeOptions={[10, 20, 50]}
      disableColumnResize
      density="compact"
      slotProps={{
        filterPanel: {
          filterFormProps: {
            logicOperatorInputProps: {
              variant: 'outlined',
              size: 'small',
            },
            columnInputProps: {
              variant: 'outlined',
              size: 'small',
              sx: { mt: 'auto' },
            },
            operatorInputProps: {
              variant: 'outlined',
              size: 'small',
              sx: { mt: 'auto' },
            },
            valueInputProps: {
              InputComponentProps: {
                variant: 'outlined',
                size: 'small',
              },
            },
          },
        },
      }}
    />
  );
}

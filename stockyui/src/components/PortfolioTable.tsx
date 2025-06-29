import React, { useEffect, useState } from 'react';
import { DataGrid, GridColDef } from '@mui/x-data-grid';
import { Dialog, DialogTitle, DialogContent, DialogActions, Button, Typography, Alert } from '@mui/material';
import { usePortfolio } from '../hooks/usePortfolio';
import { StockyApi } from '../services/generated/stockyapi';

export default function PortfolioTable() {
  const { items, isLoading, error, deleteTicker } = usePortfolio();
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [selectedItem, setSelectedItem] = useState<StockyApi.PortfolioItem | null>(null);
  const [deleteLoading, setDeleteLoading] = useState(false);
  const [deleteError, setDeleteError] = useState<string | null>(null);

  const handleDeleteClick = (item: StockyApi.PortfolioItem) => {
    setSelectedItem(item);
    setDeleteDialogOpen(true);
    setDeleteError(null);
  };

  const handleDeleteConfirm = async () => {
    if (!selectedItem) return;

    setDeleteLoading(true);
    setDeleteError(null);

    try {
      const result = await deleteTicker(selectedItem.symbol || '');

      if (result.success) {
        setDeleteDialogOpen(false);
        setSelectedItem(null);
      } else {
        setDeleteError(result.message || 'Failed to delete ticker');
      }
    } catch (error) {
      setDeleteError('An unexpected error occurred');
    } finally {
      setDeleteLoading(false);
    }
  };

  const handleDeleteCancel = () => {
    setDeleteDialogOpen(false);
    setSelectedItem(null);
    setDeleteError(null);
  };

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
      lastUpdatedTime: item.lastUpdatedTime || '',
      // Store the original item for delete operations
      originalItem: item
    }));
  }, [items]);

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
            onClick={() => handleDeleteClick(params.row.originalItem)}
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

  if (isLoading) {
    return <div>Loading portfolio data...</div>;
  }

  if (error) {
    return <div>Error loading portfolio: {error}</div>;
  }

  return (
    <>
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

      {/* Delete Confirmation Dialog */}
      <Dialog open={deleteDialogOpen} onClose={handleDeleteCancel} maxWidth="sm" fullWidth>
        <DialogTitle>Delete Portfolio Item</DialogTitle>
        <DialogContent>
          {deleteError && (
            <Alert severity="error" sx={{ mb: 2 }}>
              {deleteError}
            </Alert>
          )}
          <Typography>
            Are you sure you want to delete <strong>{selectedItem?.symbol}</strong> from your portfolio?
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
            This will permanently remove {selectedItem?.quantity} shares of {selectedItem?.symbol} from your portfolio.
            This action cannot be undone.
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleDeleteCancel} disabled={deleteLoading}>
            Cancel
          </Button>
          <Button
            onClick={handleDeleteConfirm}
            color="error"
            variant="contained"
            disabled={deleteLoading}
          >
            {deleteLoading ? 'Deleting...' : 'Delete'}
          </Button>
        </DialogActions>
      </Dialog>
    </>
  );
}

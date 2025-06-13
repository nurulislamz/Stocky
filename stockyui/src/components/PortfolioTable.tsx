import * as React from 'react';
import { DataGrid, GridColDef } from '@mui/x-data-grid';

const columns: GridColDef[] = [
  { field: 'symbol', headerName: 'Symbol', width: 100 },
  { field: 'companyName', headerName: 'Company', width: 200 },
  { field: 'shares', headerName: 'Shares', width: 100, type: 'number' },
  {
    field: 'avgPrice',
    headerName: 'Avg Price',
    width: 120,
    type: 'number',
    valueFormatter: ({ value }: { value: number }) => `$${value.toFixed(2)}`
  },
  {
    field: 'currentPrice',
    headerName: 'Current Price',
    width: 120,
    type: 'number',
    valueFormatter: ({ value }: { value: number }) => `$${value.toFixed(2)}`
  },
  {
    field: 'totalValue',
    headerName: 'Total Value',
    width: 120,
    type: 'number',
    valueFormatter: ({ value }: { value: number }) => `$${value.toFixed(2)}`
  },
  {
    field: 'profitLoss',
    headerName: 'P/L',
    width: 120,
    type: 'number',
    valueFormatter: ({ value }: { value: number }) => `$${value.toFixed(2)}`,
    cellClassName: (params) => params.value >= 0 ? 'positive' : 'negative'
  },
  {
    field: 'profitLossPercent',
    headerName: 'P/L %',
    width: 100,
    type: 'number',
    valueFormatter: ({ value }: { value: number }) => `${value.toFixed(2)}%`,
    cellClassName: (params) => params.value >= 0 ? 'positive' : 'negative'
  },
  {
    field: 'actions',
    headerName: 'Actions',
    width: 120,
    sortable: false,
    renderCell: (params) => (
      <div style={{ display: 'flex', gap: '8px' }}>
        <button
          onClick={() => console.log('Edit clicked for id:', params.row.id)}
          style={{
            border: 'none',
            background: 'none',
            color: '#1976d2',
            cursor: 'pointer',
            padding: '4px'
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
            padding: '4px'
          }}
        >
          Delete
        </button>
      </div>
    ),
  },
];

export default function PortfolioTable() {
  const [rows, setRows] = React.useState<any[]>([]);

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

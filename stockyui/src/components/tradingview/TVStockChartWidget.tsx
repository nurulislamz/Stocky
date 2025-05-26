import { memo } from 'react';
import TVWidget from './TVWidget';

interface TVStockChartWidgetProps {
  symbol: string;
  theme?: 'light' | 'dark';
  interval?: string;
  locale?: string;
  autosize?: boolean;
  width?: string | null;
  height?: string | null;
  allowSymbolChange?: boolean;
}

const TVStockChartWidget = (widgetProps: TVStockChartWidgetProps) => {
  return (
    <TVWidget
      scriptSrc="https://s3.tradingview.com/external-embedding/embed-widget-advanced-chart.js"
      widgetConfig={widgetProps}
    />
  );
};

export default memo(TVStockChartWidget);
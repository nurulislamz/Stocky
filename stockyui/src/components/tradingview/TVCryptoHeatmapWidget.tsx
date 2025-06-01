import { memo } from 'react';
import TVWidget from './TVWidget';

export interface TVCrytoHeatmapWidgetProps {
  dataSource: string,
  blockSize: string,
  blockColor: string,
  locale: string,
  symbolUrl: string,
  colorTheme: string,
  hasTopBar: boolean,
  isDataSetEnabled: boolean,
  isZoomEnabled: boolean,
  hasSymbolTooltip: boolean,
  isMonoSize: boolean,
  width: string,
  height: string
}

const TVCrytoHeatmapWidget = (widgetProps: TVCrytoHeatmapWidgetProps) => {
  return (
    <TVWidget
      scriptSrc="https://s3.tradingview.com/external-embedding/embed-widget-crypto-coins-heatmap.js"
      widgetConfig={widgetProps}
    />
  );
};

export default memo(TVCrytoHeatmapWidget);
import { memo } from 'react';
import TVWidget from './TVWidget';

export interface TVSP500HeatmapWidgetProps {
  exchanges: string[],
  dataSource: string,
  grouping: string,
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

const TVSP500HeatmapWidget = (widgetProps: TVSP500HeatmapWidgetProps) => {
  return (
    <TVWidget
      scriptSrc="https://s3.tradingview.com/external-embedding/embed-widget-stock-heatmap.js"
      widgetConfig={widgetProps}
    />
  );
};

export default memo(TVSP500HeatmapWidget);
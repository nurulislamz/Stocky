import React, { useEffect, useRef, memo } from 'react';

interface TVWidgetProps {
  scriptSrc: string;
  widgetConfig: object;
}

function TVWidget({ scriptSrc, widgetConfig }: TVWidgetProps) {
  const container = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const currentContainer = container.current;

    if (!currentContainer) {
      console.warn("Container is not available.");
      return;
    }

    // Only initialize once
      const script = document.createElement("script");
      script.src = scriptSrc;
      script.type = "text/javascript";
      script.async = true;

      script.innerHTML = JSON.stringify(widgetConfig);

      // Append the script and handle errors
      const timeoutId = setTimeout(() => {
        try {
          currentContainer.appendChild(script);
        } catch (error) {
          console.error("Failed to load TradingView widget script:", error);
        }
      }, 100);

      return () => {
        clearTimeout(timeoutId);
        if (currentContainer) {
          while (currentContainer.firstChild) {
            currentContainer.removeChild(currentContainer.firstChild);
          }
        }
      };
  }, [scriptSrc, widgetConfig]);

  return (
    <div
      className="tradingview-widget-container"
      ref={container}
      style={{ height: "100%", width: "100%" }}
    >
      <div
        className="tradingview-widget-container__widget"
        style={{ height: "100%", width: "100%" }}
      ></div>
    </div>
  );
}

export default memo(TVWidget);
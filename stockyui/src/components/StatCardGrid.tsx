import Grid from "@mui/material/Grid";
import StatCard, { StatCardProps } from "./StatCard";
import HighlightedCard from './HighlightedCard';

export default function StatCardGrid({ data }: { data : StatCardProps[] }) {
  return (
    data.length > 0 ? (
      <>
        {data.map((card, index) => (
          <Grid key={index} size={{ xs: 12, sm: 6, lg: 3 }}>
            <StatCard {...card} />
          </Grid>
        ))}
        <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
          <HighlightedCard />
        </Grid>
      </>
    ) : (
      null
    )
  );
}
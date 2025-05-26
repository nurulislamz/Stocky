import { useSearchParams } from 'react-router-dom';
import Header from '../../components/Header';
import Layout from "../../templates/Layout";
import SearchHero from "./SearchHero";
import SearchResults from "./SearchResults";

export default function SearchPage() {
  const [searchParams] = useSearchParams();
  const query = searchParams.get('q');

  return (
    <Layout>
      <Header headerName="Search" />
      {query ? <SearchResults symbol={query} /> : <SearchHero />}
    </Layout>
  );
}


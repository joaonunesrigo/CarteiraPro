import { useToastContext } from '../../components/Toast'
import {
  useAdicionarAtivo,
  useExcluirAtivo,
} from '../../features/carteira/hooks/useAtivos'
import { useCarteira } from '../../features/carteira/hooks/useCarteira'
import { Dashboard } from './Dashboard'

export default function DashboardPage() {
  const { mostrarToast } = useToastContext()
  const { cartoesResumo, linhasAtivos, dadosGraficos, carregando, erro, recarregar } =
    useCarteira()

  const tickersCadastrados = linhasAtivos.map((linha) => linha.ticker)
  const formularioAtivo = useAdicionarAtivo(
    recarregar,
    tickersCadastrados,
    mostrarToast,
  )
  const { excluirAtivo } = useExcluirAtivo(recarregar, mostrarToast)

  return (
    <Dashboard
      cartoesResumo={cartoesResumo}
      linhasAtivos={linhasAtivos}
      dadosGraficos={dadosGraficos}
      carregando={carregando}
      erro={erro}
      formularioAtivo={formularioAtivo}
      excluirAtivo={excluirAtivo}
    />
  )
}

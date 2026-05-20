import { useConfirmDialogContext } from '../../components/ConfirmDialog'
import { useTabs } from '../../components/Tabs'
import { useToastContext } from '../../components/Toast'
import {
  useAdicionarAtivo,
  useExcluirAtivo,
} from '../../features/carteira/hooks/useAtivos'
import { useImportarB3 } from '../../features/carteira/hooks/useImportarB3'
import { useCarteira } from '../../features/carteira/hooks/useCarteira'
import { useImportarMovimentacaoB3 } from '../../features/proventos/hooks/useImportarMovimentacaoB3'
import { useProventos } from '../../features/proventos/hooks/useProventos'
import { ABA_INICIAL_DASHBOARD, ABAS_DASHBOARD } from './dashboardAbas.constants'
import { Dashboard } from './Dashboard'

export default function DashboardPage() {
  const { mostrarToast } = useToastContext()
  const { solicitarConfirmacao } = useConfirmDialogContext()
  const { abaAtiva, mudarAba } = useTabs(ABA_INICIAL_DASHBOARD)
  const { cartoesResumo, linhasAtivos, dadosGraficos, carregando, erro, recarregar } =
    useCarteira()

  const tickersCadastrados = linhasAtivos.map((linha) => linha.ticker)
  const formularioAtivo = useAdicionarAtivo(
    recarregar,
    tickersCadastrados,
    mostrarToast,
  )
  const { excluirAtivo, excluirTodosAtivos } = useExcluirAtivo(
    recarregar,
    mostrarToast,
    solicitarConfirmacao,
  )
  const importadorB3 = useImportarB3(recarregar, mostrarToast)
  const proventos = useProventos()
  const importadorMovimentacaoB3 = useImportarMovimentacaoB3(
    proventos.recarregar,
    mostrarToast,
  )

  return (
    <Dashboard
      abas={ABAS_DASHBOARD}
      abaAtiva={abaAtiva}
      onMudarAba={mudarAba}
      cartoesResumo={cartoesResumo}
      linhasAtivos={linhasAtivos}
      dadosGraficos={dadosGraficos}
      carregando={carregando}
      erro={erro}
      formularioAtivo={formularioAtivo}
      importadorB3={importadorB3}
      painelProventos={{
        ...proventos,
        importador: importadorMovimentacaoB3,
      }}
      excluirAtivo={excluirAtivo}
      excluirTodosAtivos={excluirTodosAtivos}
    />
  )
}

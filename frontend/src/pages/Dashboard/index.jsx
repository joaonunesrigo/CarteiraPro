import { useState } from 'react'
import { useConfirmDialogContext } from '../../components/ConfirmDialog'
import { useTabs } from '../../components/Tabs'
import { useToastContext } from '../../components/Toast'
import { useAdicionarAtivo, useExcluirAtivo } from '../../features/carteira/hooks/useAtivos'
import { useImportarB3 } from '../../features/carteira/hooks/useImportarB3'
import { useCarteira } from '../../features/carteira/hooks/useCarteira'
import { useCarteiras } from '../../features/carteira/hooks/useCarteiras'
import { useDesempenhoCarteira } from '../../features/carteira/hooks/useDesempenhoCarteira'
import { useOperacoesAtivo } from '../../features/carteira/hooks/useOperacoesAtivo'
import { useImportarMovimentacaoB3 } from '../../features/proventos/hooks/useImportarMovimentacaoB3'
import { useProventos } from '../../features/proventos/hooks/useProventos'
import { ABA_INICIAL_DASHBOARD, ABAS_DASHBOARD } from './dashboardAbas.constants'
import { Dashboard } from './Dashboard'

export default function DashboardPage({ usuario, logout }) {
  const { mostrarToast } = useToastContext()
  const { solicitarConfirmacao } = useConfirmDialogContext()
  const { abaAtiva, mudarAba } = useTabs(ABA_INICIAL_DASHBOARD)
  const carteiras = useCarteiras(mostrarToast, solicitarConfirmacao)
  const { cartoesResumo, linhasAtivos, dadosGraficos, carregando, erro, cotacaoAtualizadaEm } = useCarteira()

  const [adicionarAtivoAberto, setAdicionarAtivoAberto] = useState(false)
  const [importadorB3Aberto, setImportadorB3Aberto] = useState(false)

  const tickersCadastrados = linhasAtivos.map((linha) => linha.ticker)
  const formularioAtivo = useAdicionarAtivo(tickersCadastrados, mostrarToast, () => setAdicionarAtivoAberto(false))
  const { excluirAtivo, excluirTodosAtivos } = useExcluirAtivo(mostrarToast, solicitarConfirmacao)
  const importadorB3 = useImportarB3(mostrarToast, () => setImportadorB3Aberto(false))
  const operacoesAtivo = useOperacoesAtivo(mostrarToast, solicitarConfirmacao)
  const proventos = useProventos()
  const importadorMovimentacaoB3 = useImportarMovimentacaoB3(mostrarToast)
  const desempenho = useDesempenhoCarteira()

  return (
    <Dashboard
      abas={ABAS_DASHBOARD}
      abaAtiva={abaAtiva}
      onMudarAba={mudarAba}
      cartoesResumo={cartoesResumo}
      cotacaoAtualizadaEm={cotacaoAtualizadaEm}
      linhasAtivos={linhasAtivos}
      dadosGraficos={dadosGraficos}
      desempenho={desempenho}
      carregando={carregando}
      erro={erro}
      formularioAtivo={formularioAtivo}
      importadorB3={importadorB3}
      carteiras={carteiras}
      operacoesAtivo={operacoesAtivo}
      painelProventos={{
        ...proventos,
        importador: importadorMovimentacaoB3,
      }}
      excluirAtivo={excluirAtivo}
      excluirTodosAtivos={excluirTodosAtivos}
      adicionarAtivoModal={{
        aberto: adicionarAtivoAberto,
        abrir: () => setAdicionarAtivoAberto(true),
        fechar: () => setAdicionarAtivoAberto(false),
      }}
      importadorB3Modal={{
        aberto: importadorB3Aberto,
        abrir: () => setImportadorB3Aberto(true),
        fechar: () => setImportadorB3Aberto(false),
      }}
      usuario={usuario}
      logout={logout}
    />
  )
}

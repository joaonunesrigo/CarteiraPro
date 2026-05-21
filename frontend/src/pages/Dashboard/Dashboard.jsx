import { Alert } from '../../components/Alert'
import { Button } from '../../components/Button'
import { Tabs } from '../../components/Tabs'
import { AtivoTable } from '../../features/carteira/components/AtivoTable'
import { CarteiraGraficos } from '../../features/carteira/components/CarteiraGraficos'
import { CarteiraSummary } from '../../features/carteira/components/CarteiraSummary'
import { ModalAdicionarAtivo } from '../../features/carteira/components/FormularioAtivo'
import { GraficoDesempenhoCarteira } from '../../features/carteira/components/GraficoDesempenho'
import { ModalImportadorB3 } from '../../features/carteira/components/ImportadorB3'
import { PainelOperacoesAtivo } from '../../features/carteira/components/PainelOperacoesAtivo'
import { PainelProventos } from '../../features/proventos/components/PainelProventos'

export function Dashboard({
  abas,
  abaAtiva,
  onMudarAba,
  cartoesResumo,
  cotacaoAtualizadaEm,
  linhasAtivos,
  dadosGraficos,
  desempenho,
  carregando,
  erro,
  formularioAtivo,
  importadorB3,
  operacoesAtivo,
  painelProventos,
  excluirAtivo,
  excluirTodosAtivos,
  adicionarAtivoModal,
  importadorB3Modal,
  usuario,
  logout,
}) {
  const paineis = {
    carteira: (
      <div className="space-y-6">
        <CarteiraSummary cartoes={cartoesResumo} cotacaoAtualizadaEm={cotacaoAtualizadaEm} />

        <GraficoDesempenhoCarteira {...desempenho} />

        <AtivoTable
          linhas={linhasAtivos}
          excluirAtivo={excluirAtivo}
          excluirTodosAtivos={excluirTodosAtivos}
          abrirPainelOperacoes={operacoesAtivo.abrirPainel}
          abrirAdicionarAtivo={adicionarAtivoModal.abrir}
          abrirImportadorB3={importadorB3Modal.abrir}
        />

        <PainelOperacoesAtivo {...operacoesAtivo} />
        <ModalAdicionarAtivo aberto={adicionarAtivoModal.aberto} onFechar={adicionarAtivoModal.fechar} formularioAtivo={formularioAtivo} />
        <ModalImportadorB3 aberto={importadorB3Modal.aberto} onFechar={importadorB3Modal.fechar} importadorB3={importadorB3} />
      </div>
    ),
    graficos: <CarteiraGraficos dadosGraficos={dadosGraficos} />,
    proventos: <PainelProventos {...painelProventos} />,
  }

  return (
    <div className="min-h-screen bg-slate-950 text-slate-100">
      <header className="border-b border-slate-800 px-6 py-8">
        <div className="mx-auto flex max-w-6xl flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <h1 className="text-2xl font-semibold tracking-tight text-white">CarteiraPro</h1>
            <p className="mt-1 text-slate-400">Dashboard da sua carteira</p>
          </div>

          <div className="flex items-center gap-3">
            {usuario && <span className="text-sm text-slate-400">{usuario.email}</span>}
            <Button variante="secondary" onClick={logout}>
              Sair
            </Button>
          </div>
        </div>
      </header>

      <main className="mx-auto max-w-6xl px-6 py-8">
        {erro && (
          <Alert className="mb-8">
            {erro}
            <span className="mt-1 block text-sm text-red-400/80">Verifique se a API está rodando em http://localhost:5122</span>
          </Alert>
        )}

        {carregando ? (
          <p className="text-slate-400">Carregando...</p>
        ) : (
          <Tabs abas={abas} abaAtiva={abaAtiva} onMudarAba={onMudarAba} paineis={paineis} />
        )}
      </main>
    </div>
  )
}

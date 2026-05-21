import { create } from 'zustand'

const importacaoInicial = {
  linhas: [],
  processandoArquivo: false,
  erroArquivo: null,
  nomeArquivo: null,
}

export const useProventosStore = create((set) => ({
  importacao: importacaoInicial,
  setImportacao: (dados) => set((state) => ({ importacao: { ...state.importacao, ...dados } })),
  alternarSelecionadoImportacao: (id) =>
    set((state) => ({
      importacao: {
        ...state.importacao,
        linhas: state.importacao.linhas.map((linha) =>
          linha.id === id && !linha.jaImportado ? { ...linha, selecionado: !linha.selecionado } : linha,
        ),
      },
    })),
  limparImportacao: () => set({ importacao: importacaoInicial }),
}))

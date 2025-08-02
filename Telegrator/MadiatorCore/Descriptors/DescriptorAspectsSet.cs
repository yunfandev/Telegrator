using Telegrator.Aspects;
using Telegrator.Handlers;
using Telegrator.Handlers.Components;

namespace Telegrator.MadiatorCore.Descriptors
{
    public sealed class DescriptorAspectsSet
    {
        public bool SelfPre { get; private set; }

        public bool SelfPost { get; private set; }

        public Type? TypedPre { get; private set; }

        public Type? TypedPost { get; private set; }

        public DescriptorAspectsSet(bool selfPre, Type? typedPre, bool selfPost, Type? typedPost)
        {
            SelfPre = selfPre;
            SelfPost = selfPost;
            TypedPre = typedPre;
            TypedPost = typedPost;
        }

        public async Task<Result> ExecutePre(UpdateHandlerBase handler, IHandlerContainer container)
        {
            if (SelfPre)
            {
                if (handler is not IPreProcessor preProcessor)
                    throw new InvalidOperationException();

                return await preProcessor.BeforeExecution(container);
            }

            if (TypedPre != null)
            {
                IPreProcessor preProcessor = (IPreProcessor)Activator.CreateInstance(TypedPre);
                return await preProcessor.BeforeExecution(container);
            }

            return Result.Ok();
        }

        public async Task<Result> ExecutePost(UpdateHandlerBase handler, IHandlerContainer container)
        {
            if (SelfPost)
            {
                if (handler is not IPostProcessor postProcessor)
                    throw new InvalidOperationException();

                return await postProcessor.AfterExecution(container);
            }

            if (TypedPost != null)
            {
                IPostProcessor postProcessor = (IPostProcessor)Activator.CreateInstance(TypedPost);
                return await postProcessor.AfterExecution(container);
            }

            return Result.Ok();
        }
    }
}

using HolyPOS.Components.Models;
using Supabase.Postgrest.Models;

namespace HolyPOS.Components.Services;

public class DatabaseData
{
    private readonly SupabaseService _supabase;

    public DatabaseData(SupabaseService supabase)
    {
        _supabase = supabase;
    }

    // ============================================================
    // GLOBAL DATA
    // ============================================================

    public List<Product> Products { get; private set; } = new();

    public List<Category> Categories { get; private set; } = new();

    public List<Store> Stores { get; private set; } = new();


    public List<Modifier> Modifiers { get; private set; } = new();

    public List<ModifierOption> ModifierOptions { get; private set; } = new();

    public List<Discount> Discounts { get; private set; } = new();

    public List<PaymentType> PaymentTypes { get; private set; } = new();

    public List<ProductStore> ProductStores { get; private set; } = new();

    public List<ProductModifier> ProductModifiers { get; private set; } = new();
    
    public List<Transaction> Transactions { get; private set; } = new();
    public List<TransactionItem> TransactionItems { get; private set; } = new();
    public List<TransactionPayment> TransactionPayments { get; private set; } = new();

    public List<TransactionItemModifier> TransactionItemModifiers { get; set; } = new();
    // ============================================================
    // LOAD EVERYTHING
    // ============================================================

    public async Task LoadAllAsync()
    {
        Products = await GetTableAsync<Product>();

        Categories = await GetTableAsync<Category>();

        Stores = await GetTableAsync<Store>();

        ProductStores = await GetTableAsync<ProductStore>();

        ProductModifiers = await GetTableAsync<ProductModifier>();

        Modifiers = await GetTableAsync<Modifier>();

        ModifierOptions = await GetTableAsync<ModifierOption>();

        Discounts = await GetTableAsync<Discount>();

        PaymentTypes = await GetTableAsync<PaymentType>();
        
        Transactions = await GetTableAsync<Transaction>();
        TransactionItems = await GetTableAsync<TransactionItem>();
        TransactionPayments = await GetTableAsync<TransactionPayment>();
        TransactionItemModifiers =
    await GetTableAsync<TransactionItemModifier>();
    }

    // ============================================================
    // GENERIC SUPABASE GET
    // ============================================================

    private async Task<List<T>> GetTableAsync<T>() where T : BaseModel, new()
    {
        var response = await _supabase.Client.From<T>().Get();

        return response.Models;
    }

    // ============================================================
    // REFRESH INDIVIDUAL TABLES
    // ============================================================

    public async Task RefreshProductsAsync()
    {
        Products = await GetTableAsync<Product>();
    }

    public async Task RefreshCategoriesAsync()
    {
        Categories = await GetTableAsync<Category>();
    }

    public async Task RefreshStoresAsync()
    {
        Stores = await GetTableAsync<Store>();
    }

    public async Task RefreshModifiersAsync()
    {
        Modifiers = await GetTableAsync<Modifier>();

        ModifierOptions = await GetTableAsync<ModifierOption>();
    }

    public async Task RefreshDiscountsAsync()
    {
        Discounts = await GetTableAsync<Discount>();
    }

    public async Task RefreshPaymentTypesAsync()
    {
        PaymentTypes = await GetTableAsync<PaymentType>();
    }

    // ============================================================
// SAVE PRODUCT
// ============================================================

    public async Task SaveProductAsync(
        Product product,
        IEnumerable<Guid> selectedStoreIds,
        Dictionary<Guid, decimal> storePrices,
        IEnumerable<Guid> selectedModifierIds)
    {
        try
        {
            // ============================================
            // PRODUCT
            // ============================================

            if (product.Id == Guid.Empty)
            {
                product.Id = Guid.NewGuid();

                await _supabase.Client
                    .From<Product>()
                    .Insert(product);
            }
            else
            {
                await _supabase.Client
                    .From<Product>()
                    .Update(product);
            }


            // ============================================
            // PRODUCT STORES
            // ============================================

            await _supabase.Client
                .From<ProductStore>()
                .Where(x => x.ProductId == product.Id)
                .Delete();


            foreach (var storeId in selectedStoreIds)
            {
                decimal? priceOverride = null;

                if (storePrices.TryGetValue(storeId, out var price))
                    if (price != product.Price)
                        priceOverride = price;

                var productStore = new ProductStore
                {
                    ProductId = product.Id,
                    StoreId = storeId,
                    IsActive = true,
                    PriceOverride = priceOverride
                };

                await _supabase.Client
                    .From<ProductStore>()
                    .Insert(productStore);
            }


            // ============================================
            // PRODUCT MODIFIERS
            // ============================================

            await _supabase.Client
                .From<ProductModifier>()
                .Where(x => x.ProductId == product.Id)
                .Delete();


            var sortOrder = 0;

            foreach (var modifierId in selectedModifierIds)
            {
                var productModifier = new ProductModifier
                {
                    ProductId = product.Id,
                    ModifierId = modifierId,
                    SortOrder = sortOrder++
                };

                await _supabase.Client
                    .From<ProductModifier>()
                    .Insert(productModifier);
            }


            // ============================================
            // REFRESH CACHE
            // ============================================

            await RefreshProductsAsync();
            await RefreshProductStoresAsync();
            await RefreshProductModifiersAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(
                $"Failed to save product: {ex.Message}",
                ex);
        }
    }

    public async Task RefreshProductStoresAsync()
    {
        ProductStores = await GetTableAsync<ProductStore>();
    }

    public async Task RefreshProductModifiersAsync()
    {
        ProductModifiers = await GetTableAsync<ProductModifier>();
    }

    // ============================================================
// DELETE PRODUCT
// ============================================================

    public async Task DeleteProductAsync(Guid productId)
    {
        if (productId == Guid.Empty)
            return;


        // ============================================
        // SUPABASE
        // ============================================

        await _supabase.Client
            .From<Product>()
            .Where(x => x.Id == productId)
            .Delete();


        // ============================================
        // LOCAL CACHE
        // ============================================

        Products.RemoveAll(
            x => x.Id == productId);

        ProductStores.RemoveAll(
            x => x.ProductId == productId);

        ProductModifiers.RemoveAll(
            x => x.ProductId == productId);
    }

    // ============================================================
// SAVE CATEGORY
// ============================================================

    public async Task SaveCategoryAsync(Category category)
    {
        if (category.Id == Guid.Empty)
        {
            category.Id = Guid.NewGuid();

            // SUPABASE: Insert new category
            await _supabase.Client
                .From<Category>()
                .Insert(category);
        }
        else
        {
            // SUPABASE: Update existing category
            await _supabase.Client
                .From<Category>()
                .Update(category);
        }


        // ============================================
        // UPDATE LOCAL CACHE
        // ============================================

        var existing = Categories
            .FirstOrDefault(x => x.Id == category.Id);

        if (existing == null)
        {
            Categories.Add(category);
        }
        else
        {
            var index = Categories.IndexOf(existing);

            Categories[index] = category;
        }
    }


// ============================================================
// DELETE CATEGORY
// ============================================================

    public async Task DeleteCategoryAsync(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
            return;


        // ============================================
        // SUPABASE: Delete category
        // ============================================

        await _supabase.Client
            .From<Category>()
            .Where(x => x.Id == categoryId)
            .Delete();


        // ============================================
        // UPDATE LOCAL CACHE
        // ============================================

        Categories.RemoveAll(
            x => x.Id == categoryId);
    }

    // ============================================================
// SAVE DISCOUNT
// ============================================================

    public async Task SaveDiscountAsync(Discount discount)
    {
        if (discount.Id == Guid.Empty)
        {
            discount.Id = Guid.NewGuid();

            // SUPABASE: Insert new discount
            await _supabase.Client
                .From<Discount>()
                .Insert(discount);
        }
        else
        {
            // SUPABASE: Update existing discount
            await _supabase.Client
                .From<Discount>()
                .Update(discount);
        }


        // ========================================================
        // UPDATE LOCAL CACHE
        // ========================================================

        var existing = Discounts
            .FirstOrDefault(x => x.Id == discount.Id);

        if (existing == null)
        {
            Discounts.Add(discount);
        }
        else
        {
            var index = Discounts.IndexOf(existing);

            Discounts[index] = discount;
        }
    }


// ============================================================
// DELETE DISCOUNT
// ============================================================

    public async Task DeleteDiscountAsync(Guid discountId)
    {
        if (discountId == Guid.Empty)
            return;


        // SUPABASE: Delete discount

        await _supabase.Client
            .From<Discount>()
            .Where(x => x.Id == discountId)
            .Delete();


        // ========================================================
        // UPDATE LOCAL CACHE
        // ========================================================

        Discounts.RemoveAll(
            x => x.Id == discountId);
    }

    // ============================================================
// SAVE MODIFIER
// ============================================================

    public async Task SaveModifierAsync(
        Modifier modifier,
        IEnumerable<ModifierOption> options)
    {
        if (modifier.Id == Guid.Empty)
        {
            modifier.Id = Guid.NewGuid();

            // ========================================
            // SUPABASE: INSERT MODIFIER
            // ========================================

            await _supabase.Client
                .From<Modifier>()
                .Insert(modifier);
        }
        else
        {
            // ========================================
            // SUPABASE: UPDATE MODIFIER
            // ========================================

            await _supabase.Client
                .From<Modifier>()
                .Update(modifier);


            // ========================================
            // DELETE EXISTING OPTIONS
            // ========================================

            await _supabase.Client
                .From<ModifierOption>()
                .Where(x =>
                    x.ModifierId == modifier.Id)
                .Delete();
        }


        // ========================================
        // INSERT OPTIONS
        // ========================================

        var optionList = options
            .ToList();

        for (var i = 0; i < optionList.Count; i++)
        {
            var option = optionList[i];

            option.Id = Guid.NewGuid();

            option.ModifierId = modifier.Id;

            option.SortOrder = i;

            await _supabase.Client
                .From<ModifierOption>()
                .Insert(option);
        }


        // ========================================
        // UPDATE LOCAL CACHE
        // ========================================

        var existingModifier = Modifiers
            .FirstOrDefault(x =>
                x.Id == modifier.Id);

        if (existingModifier is null)
        {
            Modifiers.Add(modifier);
        }
        else
        {
            var index = Modifiers
                .IndexOf(existingModifier);

            Modifiers[index] = modifier;
        }


        // Replace options in local cache

        ModifierOptions.RemoveAll(
            x => x.ModifierId == modifier.Id);

        ModifierOptions.AddRange(optionList);
    }


// ============================================================
// DELETE MODIFIER
// ============================================================

    public async Task DeleteModifierAsync(
        Guid modifierId)
    {
        if (modifierId == Guid.Empty)
            return;


        // ========================================
        // DELETE OPTIONS
        // ========================================
        //
        // This is safe even if the database also
        // has ON DELETE CASCADE.
        //

        await _supabase.Client
            .From<ModifierOption>()
            .Where(x =>
                x.ModifierId == modifierId)
            .Delete();


        // ========================================
        // DELETE MODIFIER
        // ========================================

        await _supabase.Client
            .From<Modifier>()
            .Where(x =>
                x.Id == modifierId)
            .Delete();


        // ========================================
        // UPDATE LOCAL CACHE
        // ========================================

        Modifiers.RemoveAll(
            x => x.Id == modifierId);

        ModifierOptions.RemoveAll(
            x => x.ModifierId == modifierId);
    }
    
    // ============================================================
// SAVE PAYMENT TYPE
// ============================================================

public async Task SavePaymentTypeAsync(
    PaymentType paymentType)
{
    if (paymentType.Id == Guid.Empty)
    {
        // ========================================================
        // NEW PAYMENT TYPE
        // ========================================================

        paymentType.Id = Guid.NewGuid();


        // ========================================================
        // SUPABASE INSERT
        // ========================================================

        await _supabase.Client
            .From<PaymentType>()
            .Insert(paymentType);


        // ========================================================
        // LOCAL CACHE
        // ========================================================

        PaymentTypes.Add(paymentType);
    }
    else
    {
        // ========================================================
        // SUPABASE UPDATE
        // ========================================================

        await _supabase.Client
            .From<PaymentType>()
            .Where(x => x.Id == paymentType.Id)
            .Update(paymentType);


        // ========================================================
        // LOCAL CACHE
        // ========================================================

        var existing =
            PaymentTypes.FirstOrDefault(
                x => x.Id == paymentType.Id);

        if (existing is not null)
        {
            var index =
                PaymentTypes.IndexOf(existing);

            PaymentTypes[index] = paymentType;
        }
    }
}


// ============================================================
// DELETE PAYMENT TYPE
// ============================================================

public async Task DeletePaymentTypeAsync(
    Guid paymentTypeId)
{
    if (paymentTypeId == Guid.Empty)
        return;


    // ========================================================
    // SUPABASE DELETE
    // ========================================================

    await _supabase.Client
        .From<PaymentType>()
        .Where(x => x.Id == paymentTypeId)
        .Delete();


    // ========================================================
    // LOCAL CACHE
    // ========================================================

    PaymentTypes.RemoveAll(
        x => x.Id == paymentTypeId);
}

// ============================================================
// SALES SUMMARY
// ============================================================

// ============================================================
// STORES
// ============================================================

    public async Task<List<Store>> GetStoresAsync()
    {
        try
        {
            var response =
                await _supabase.Client
                    .From<Store>()
                    .Get();

            return response.Models.ToList();
        }
        catch (Exception ex)
        {
            throw new Exception(
                $"Failed to load stores: {ex.Message}",
                ex);
        }
    }


// ============================================================
// SAVE STORE
// ============================================================

    public async Task<Store> SaveStoreAsync(Store store)
    {
        try
        {
            if (store.Id == Guid.Empty)
            {
                store.Id = Guid.NewGuid();

                var response =
                    await _supabase.Client
                        .From<Store>()
                        .Insert(store);

                return response.Models.First();
            }


            var updateResponse =
                await _supabase.Client
                    .From<Store>()
                    .Where(x =>
                        x.Id == store.Id)
                    .Update(store);

            return updateResponse.Models.First();
        }
        catch (Exception ex)
        {
            throw new Exception(
                $"Failed to save store: {ex.Message}",
                ex);
        }
    }


// ============================================================
// DELETE STORE
// ============================================================

    public async Task DeactivateStoreAsync(Guid storeId)
    {
        try
        {
            await _supabase.Client
                .From<Store>()
                .Filter(
                    "id",
                    Supabase.Postgrest.Constants.Operator.Equals,
                    storeId.ToString())
                .Set(x => x.Active, false)
                .Update();
        }
        catch (Exception ex)
        {
            throw new Exception(
                $"Failed to deactivate store: {ex.Message}",
                ex);
        }
    }
    
    // ============================================================
// TRANSACTION
// ============================================================

    public async Task<Transaction> SaveTransactionAsync(
        Transaction transaction)
    {
        try
        {
            var response = await _supabase.Client
                .From<Transaction>()
                .Insert(transaction);

            return response.Models.First();
        }
        catch (Exception ex)
        {
            throw new Exception(
                $"Failed to save transaction: {ex.Message}",
                ex);
        }
    }
    
    // ============================================================
// TRANSACTION ITEM
// ============================================================

    public async Task<TransactionItem> SaveTransactionItemAsync(
        TransactionItem item)
    {
        try
        {
            var response = await _supabase.Client
                .From<TransactionItem>()
                .Insert(item);

            return response.Models.First();
        }
        catch (Exception ex)
        {
            throw new Exception(
                $"Failed to save transaction item: {ex.Message}",
                ex);
        }
    }
    
    // ============================================================
// TRANSACTION PAYMENT
// ============================================================

    public async Task<TransactionPayment> SaveTransactionPaymentAsync(
        TransactionPayment payment)
    {
        try
        {
            var response = await _supabase.Client
                .From<TransactionPayment>()
                .Insert(payment);

            return response.Models.First();
        }
        catch (Exception ex)
        {
            throw new Exception(
                $"Failed to save transaction payment: {ex.Message}",
                ex);
        }
    }
    
    // ============================================================
// MARK TRANSACTION AS REFUNDED
// ============================================================

    public async Task MarkTransactionRefundedAsync(
        Guid transactionId)
    {
        try
        {
            await _supabase.Client
                .From<Transaction>()
                .Filter(
                    "id",
                    Supabase.Postgrest.Constants.Operator.Equals,
                    transactionId.ToString())
                .Set(
                    x => x.IsRefunded,
                    true)
                .Update();
        }
        catch (Exception ex)
        {
            throw new Exception(
                $"Failed to mark transaction as refunded: {ex.Message}",
                ex);
        }
    }

    // ============================================================
// TRANSACTION ITEM MODIFIER
// ============================================================

public async Task<TransactionItemModifier>
    SaveTransactionItemModifierAsync(
        TransactionItemModifier modifier)
{
    try
    {
        var response =
            await _supabase.Client
                .From<TransactionItemModifier>()
                .Insert(modifier);

        return response.Models.First();
    }
    catch (Exception ex)
    {
        throw new Exception(
            $"Failed to save transaction item modifier: {ex.Message}",
            ex);
    }
}

public decimal GetItemUnitPrice(TransactionItem item)
{
    if (!item.ProductId.HasValue)
        return 0m;


    var product =
        Products.FirstOrDefault(
            x => x.Id == item.ProductId.Value);


    if (product is null)
        return 0m;


    // =========================================================
    // GET STORE FROM TRANSACTION
    // =========================================================

    var transaction =
        Transactions.FirstOrDefault(
            x => x.Id == item.TransactionId);


    decimal unitPrice =
        product.Price;


    // =========================================================
    // STORE PRICE OVERRIDE
    // =========================================================

    if (transaction?.StoreId.HasValue == true)
    {
        var productStore =
            ProductStores.FirstOrDefault(
                x =>
                    x.ProductId == product.Id &&
                    x.StoreId == transaction.StoreId.Value &&
                    x.IsActive);


        if (productStore?.PriceOverride.HasValue == true)
        {
            unitPrice =
                productStore.PriceOverride.Value;
        }
    }


    // =========================================================
    // ADD MODIFIERS
    // =========================================================

    var modifiers =
        TransactionItemModifiers
            .Where(x =>
                x.TransactionItemId == item.Id);


    foreach (var modifier in modifiers)
    {
        var option =
            ModifierOptions.FirstOrDefault(
                x => x.Id == modifier.OptionId);


        if (option is not null)
        {
            unitPrice += option.Price;
        }
    }


    return unitPrice;
}
}